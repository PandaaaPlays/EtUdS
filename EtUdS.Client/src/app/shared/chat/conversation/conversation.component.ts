import {
  AfterViewChecked,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  TemplateRef,
  ViewChild
} from '@angular/core';
import {Header} from '../../header/header.component';
import {FormsModule} from '@angular/forms';
import {MessageBox} from './message-box/message-box.component';
import {HttpClient} from '@angular/common/http';
import {NgForOf, NgIf} from '@angular/common';
import {AuthService} from '../../auth/auth.service';
import {formatTime, getMonthAbbreviation} from '../../../utils/dates';
import {ButtonEtuds} from '../../button-etuds/button-etuds.component';
import {DialogComponent} from '../../dialog/dialog.component';
import {MatDialog} from '@angular/material/dialog';

interface Message {
  idSender: number | null;
  nomSender: string | null;
  date: string;
  message: string;
  vues: Vue[] | null;
}

interface Etudiant {
  id: number;
  prenom: string;
  nom: string;
}

interface Vue {
  id: number;
  nom: string;
}

interface Participant {
  idEtudiant: number;
  idParticipant: number;
  prenom: string;
  nom: string;
}

interface TitreConversation {
  titre: string;
}

@Component({
  selector: 'conversation',
  standalone: true,
  templateUrl: './conversation.component.html',
  styleUrls: ['./conversation.component.css'],
  imports: [
    Header,
    FormsModule,
    MessageBox,
    NgForOf,
    NgIf,
    ButtonEtuds
  ]
})

export class Conversation {
  @Input() conversationId!: number;
  @Input() etudiants!: Etudiant[];
  @ViewChild('messagesContainer') messagesContainer!: ElementRef;
  @ViewChild('modifierConversationTemplate') modifierConversationTemplate!: TemplateRef<any>;
  @Output() deselectConversation = new EventEmitter<null>();
  messageText: string = '';
  currentEtudiantId!: number;
  titre: string = "";

  constructor(private https: HttpClient, private authService: AuthService, private dialog: MatDialog) {}

  ngOnInit() {
    this.getMessages();
    this.getParticipants();
    this.getConversation();
    this.scrollToBottom();
    this.currentEtudiantId = this.authService.getEtudiantId() ?? 0;
  }

  conversationNom: string = "Conversation";
  getConversation() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Conversations/${this.conversationId}`;

    this.https.get<TitreConversation>(url).subscribe(
      (result) => {
        this.conversationNom = result.titre;
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  participants: Participant[] = [];
  stringParticipants: string = "";
  getParticipants() {
    this.stringParticipants = "Conversation avec ";
    const host = window.location.hostname;
    let url = `https://${host}/api/Conversations/${this.conversationId}/Participants`;

    this.https.get<Participant[]>(url).subscribe(
      (result) => {
        this.participants = result.filter(p => p.idEtudiant !== this.currentEtudiantId);
        const noms = this.participants.map(p => `${p.prenom} ${p.nom}`);
        if (noms.length === 0) {
          this.stringParticipants = "Conversation seul"
        } else if (noms.length === 1) {
          this.stringParticipants += noms[0];
        } else if (noms.length === 2) {
          this.stringParticipants += noms.join(" et ");
        } else if (noms.length > 2) {
          this.stringParticipants += noms.slice(0, -1).join(", ") + " et " + noms[noms.length - 1];
        }

        this.etudiantsAjoutes = this.etudiants.filter(e =>
          this.participants.some(p => p.idEtudiant === e.id)
        );
        this.etudiantsFiltered = this.etudiants.filter(e =>
          !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
        );
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  messages: Message[] = [];
  getMessages() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Messages/${this.conversationId}`;

    this.https.get<Message[]>(url, {withCredentials: true}).subscribe(
      (result) => {
        this.messages = result.map(m => {
          let filteredVues = null;
          if(m.vues != null) {
            filteredVues =
              m.vues.filter(vue => vue.id !== this.currentEtudiantId)
                .filter(vue => vue.id !== m.idSender);
          }
          return {
            ...m,
            vues: filteredVues,
            date: this.formatDate(m.date)
          };
        });

      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  retour() {
    this.deselectConversation.emit(null);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const day = date.getDate().toString().padStart(2, '0');
    const monthAbbreviation = getMonthAbbreviation(date.getMonth());
    const formattedDate = `${day} ${monthAbbreviation} (` + formatTime(dateString) + ')';
    return formattedDate;
  }

  envoyerMessage() {
    if (this.messageText.trim()) {
      const host = window.location.hostname;
      let url = `https://${host}/api/Messages`;

      const message = {
        IdConversation: this.conversationId,
        Contenu: this.messageText
      };

      this.https.post<Message>(url, message, {withCredentials: true}).subscribe(
        (result) => {
          this.getMessages();
          this.scrollToBottom();
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
      setTimeout(() => {
        this.messageText = '';
      }, 1);
    }
  }

  hideTitle(index: number): boolean {
    if (index === 0) {
      return false;
    }

    const currentMessage = this.messages[index];
    const previousMessage = this.messages[index - 1];

    const parseCustomDate = (dateString: string): Date => {
      const [day, month, time] = dateString.split(' ');
      const [hour, minute] = time.replace(/[()]/g, '').split(':');

      const monthMap: { [key: string]: number } = {
        'janv.': 0, 'févr.': 1, 'mars': 2, 'avr.': 3, 'mai': 4, 'juin': 5,
        'juil.': 6, 'août': 7, 'sept.': 8, 'oct.': 9, 'nov.': 10, 'déc.': 11
      };

      const currentYear = new Date().getFullYear();
      const parsedDate = new Date(currentYear, monthMap[month], parseInt(day), parseInt(hour), parseInt(minute));

      return parsedDate;
    };

    const currentMessageDate = parseCustomDate(currentMessage.date);
    const previousMessageDate = parseCustomDate(previousMessage.date);

    const timeDifference = currentMessageDate.getTime() - previousMessageDate.getTime();
    const fiveMinutesInMillis = 5 * 60 * 1000;

    return currentMessage.idSender !== null && currentMessage.idSender === previousMessage.idSender && timeDifference <= fiveMinutesInMillis;
  }


  // Modification
  menuModifierConversation() {
    this.dialog.open(DialogComponent, {
      data: { titre: "Modifier une nouvelle conversation",
        template: this.modifierConversationTemplate, width: 500,
        couleurBouton: 'vert', texteBouton: 'Modifier la conversation',
        action: this.modifierConversation.bind(this) }
    });
  }

  modifierConversation() {
    const host = window.location.hostname;
    const url = `https://${host}/api/Conversations/${this.conversationId}`;
    this.https.put(url, { id: this.conversationId, titre: (this.titre && this.titre.trim()) || this.conversationNom }, { withCredentials: true })
      .subscribe(
        () => {
          this.updateParticipants();
        },
        (error) => console.error("Erreur:", error)
      );
  }

  updateParticipants() {
    const host = window.location.hostname;
    const url = `https://${host}/api/Conversations/${this.conversationId}/participants`;
    const etudiantIds = this.etudiantsAjoutes.map(e => e.id);

    this.https.put(url, etudiantIds, { withCredentials: true })
      .subscribe(
        () => {
          this.getConversation();
          this.getParticipants();
          this.getMessages();
          this.scrollToBottom();
        },
        (error) => {
          console.error("Erreur:", error);
        }
      );
  }

  etudiantsAjoutes: Etudiant[] = [];
  etudiantSelectionne: Etudiant | null = null;
  etudiantsFiltered: Etudiant[] = [];
  ajouterParticipant() {
    if (this.etudiantSelectionne && !this.etudiantsAjoutes.some(e => e.id === this.etudiantSelectionne!.id)) {
      this.etudiantsAjoutes.push(this.etudiantSelectionne);
      this.etudiantsFiltered = this.etudiants.filter(e =>
        !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
      );
      this.etudiantSelectionne = null;
    }
  }

  supprimerParticipant(participant: Etudiant) {
    this.etudiantsAjoutes = this.etudiantsAjoutes.filter(e => e.id !== participant.id);
    this.etudiantsFiltered = this.etudiants.filter(e =>
      !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
    );
  }

  quitterPrompt() {
    this.dialog.open(DialogComponent, {
    data: { titre: "Quitter la conversation",
        message: "Êtes-vous certain de vouloir quitter la conversation?",
        couleurBouton: 'rouge', texteBouton: 'Quitter la conversation',
        action: this.quitterConversation.bind(this) }
    });
  }

  quitterConversation() {
    const host = window.location.hostname;
    const url = `https://${host}/api/Conversations/${this.conversationId}/quitter`;
    const etudiantIds = this.etudiantsAjoutes
      .filter(e => e.id !== this.currentEtudiantId)
      .map(e => e.id);

    this.https.put(url, etudiantIds, { withCredentials: true })
      .subscribe(
        () => {
          this.deselectConversation.emit(null);
          this.dialog.closeAll();
        },
        (error) => {
          console.error("Erreur:", error);
        }
      );
  }

  scrollToBottom() {
    setTimeout(() => {
      if (this.messagesContainer) {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      }
    }, 100);
  }
}
