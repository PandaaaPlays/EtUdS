import {Component, HostListener, TemplateRef, ViewChild} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {Header} from '../header/header.component';
import {ButtonEtuds} from '../button-etuds/button-etuds.component';
import {DialogComponent} from '../dialog/dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {FormsModule} from '@angular/forms';
import {HttpClient} from '@angular/common/http';
import {AuthService} from '../auth/auth.service';
import {formatTime, getMonthAbbreviation} from '../../utils/dates';
import {ConversationButton} from './conversation-button/conversation-button.component';
import {Conversation} from './conversation/conversation.component';

interface Etudiant {
  id: number;
  prenom: string;
  nom: string;
}

interface ConversationCourt {
  id: number;
  nom: string;
  prenom: string;
  message: string;
  date: string;
  nonLus: boolean;
}

@Component({
  selector: 'chat',
  standalone: true,
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  imports: [
    NgIf,
    Header,
    ButtonEtuds,
    NgForOf,
    FormsModule,
    ConversationButton,
    Conversation
  ]
})

export class Chat {
  @ViewChild('ajouterConversationTemplate') ajouterConversationTemplate!: TemplateRef<any>;
  chatVisible: boolean = false;
  notifications: number = 0;
  currentEtudiantId!: number;
  selectedConversationId: number | null = null;
  titre!: string;

  constructor(private dialog: MatDialog, private authService: AuthService, private https: HttpClient) {}

  ngOnInit() {
    this.getConversations();
    this.getEtudiants();
    this.currentEtudiantId = this.authService.getEtudiantId() ?? 0;
  }

  toggleChatVisibility(event: Event) {
    this.chatVisible = !this.chatVisible;
    event.stopPropagation();
  }

  stopPropagation(event: Event) {
    event.stopPropagation();
  }

  conversations: ConversationCourt[] = [];
  getConversations() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Conversations`;

    this.https.get<ConversationCourt[]>(url, {withCredentials: true}).subscribe(
      (result) => {
        this.conversations = result
          .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

        this.conversations = this.conversations.map(c => ({
          ...c,
          date: this.formatDate(c.date)
        }));

        this.notifications = this.conversations.filter(x => x.nonLus).length;
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const day = date.getDate().toString().padStart(2, '0');
    const monthAbbreviation = getMonthAbbreviation(date.getMonth());
    return `${day} ${monthAbbreviation} (` + formatTime(dateString) + ')';
  }

  @HostListener('document:click', ['$event'])
  closeChatOnClickOutside(event: MouseEvent) {
    const target = event.target as Element;

    if (this.chatVisible && !target.closest('.chat') && !target.closest('.chat-button') && !target.closest('.dialog')) {
      this.chatVisible = false;
    }
  }

  ajouterConversation() {
    this.dialog.open(DialogComponent, {
      data: { titre: "Ajouter une nouvelle conversation",
        template: this.ajouterConversationTemplate, width: 500,
        couleurBouton: 'vert', texteBouton: 'Créer la conversation',
        action: this.creerConversation.bind(this) }
    });
  }

  // L'étudiant actuel (connecté) est ajouté lors de la création.
  creerConversation() {
    if(this.etudiantsAjoutes == null || this.etudiantsAjoutes.length == 0) {
      this.dialog.open(DialogComponent, {
        data: { titre: "Erreur dans la conversation",
          couleurBouton: 'rouge',
          message: 'Veuillez choisir au moins un étudiant à ajouter dans la conversation.'}
      });
      return;
    }

    if(this.titre == null || this.titre == '') {
      this.dialog.open(DialogComponent, {
        data: { titre: "Erreur dans la conversation",
          couleurBouton: 'rouge',
          message: 'Veuillez choisir un titre pour la conversation.'}
      });
      return;
    }

    const host = window.location.hostname;
    const url = `https://${host}/api/Conversations`;

    this.https.post<{ id: number }>(url, { titre: this.titre }, { withCredentials: true })
      .subscribe(
        (result) => {
          if (result && result.id) {
            this.ajouterParticipants(result.id);
          }
          this.getConversations();
        },
        (error) => {
          console.error('Erreur lors de la création de la conversation:', error);
        }
      );
  }

  ajouterParticipants(conversationId: number) {
    const host = window.location.hostname;
    const url = `https://${host}/api/Conversations/${conversationId}/participants`;
    const etudiantIds = this.etudiantsAjoutes.map(e => e.id);

    this.https.post(url, etudiantIds, { withCredentials: true })
      .subscribe(
        () => {
          this.etudiantsAjoutes = [];
          this.etudiantsFiltered = this.etudiants.filter(e =>
            !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
          );
        },
        (error) => {
          console.error("Erreur:", error);
        }
      );
  }

  etudiants: Etudiant[] = [];
  getEtudiants() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Etudiants`;

    this.https.get<Etudiant[]>(url).subscribe(
      (result) => {
        this.etudiants = result;
        this.etudiantsFiltered = this.etudiants.filter(e =>
          !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
        );
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  etudiantsAjoutes: Etudiant[] = [];
  etudiantSelectionne: Etudiant | null = null;
  etudiantsFiltered: Etudiant[] = [];
  ajouterEtudiant() {
    if (this.etudiantSelectionne && !this.etudiantsAjoutes.some(e => e.id === this.etudiantSelectionne!.id)) {
      this.etudiantsAjoutes.push(this.etudiantSelectionne);
      this.etudiantsFiltered = this.etudiants.filter(e =>
        !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
      );
      this.etudiantSelectionne = null;
    }
  }

  supprimerEtudiant(etudiant: Etudiant) {
    this.etudiantsAjoutes = this.etudiantsAjoutes.filter(e => e.id !== etudiant.id);
    this.etudiantsFiltered = this.etudiants.filter(e =>
      !this.etudiantsAjoutes.some(a => a.id === e.id) && this.currentEtudiantId !== e.id
    );
  }

  deselectConversation() {
    this.selectedConversationId = null;
    this.getConversations();
  }
}
