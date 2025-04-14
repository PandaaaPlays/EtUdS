import {Component, Input, TemplateRef, ViewChild} from '@angular/core';
import {Header} from '../../../../shared/header/header.component';
import {Membre} from './membre/membre.component';
import {FormsModule} from '@angular/forms';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {HttpClient} from '@angular/common/http';
import {AuthService} from '../../../../shared/auth/auth.service';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../../../../shared/dialog/dialog.component';
import {Documents} from './documents/documents.component';
import {ButtonEtuds} from '../../../../shared/button-etuds/button-etuds.component';

interface Etudiant {
  id: string;
  prenom: string;
  nom: string;
  courriel: string;
}

interface Equipe {
  id: number;
  idDevoir: number;
  etatSoumission: number;
}

interface MembreEtudiant {
  membreId: number;
  accepte: boolean;
  etudiantId: number;
  prenom: string;
  nom: string;
  courriel: string;
}

interface MembreInvitation {
  equipeId: number;
  membres: MembreEtudiant[];
}

interface MembreEtudiant {
  id: number;
  idEtudiant: number;
  idEquipe: number;
  accepte: boolean;
}

interface Devoir {
  id: number;
  sigle: number;
  dateLimiteSoumission: string;
  nom: string;
  etat: number;
  note: number | null;
  tailleMaxEquipes: number;
}

enum EtatDevoir {
  A_Faire = 1,
  En_Retard = 2,
  Soumis = 3,
  Corrige = 4
}

interface EquipeDetailsMembres {
  equipe: Equipe;
  etudiants: MembreEtudiant[];
}

@Component({
  selector: 'devoir-equipe',
  standalone: true,

  templateUrl: './devoir-equipe.component.html',
  imports: [
    Header,
    Membre,
    FormsModule,
    NgForOf,
    NgIf,
    NgClass,
    Documents,
    ButtonEtuds
  ],
  styleUrl: './devoir-equipe.component.css'
})
export class DevoirEquipe  {
  @Input() devoir!: Devoir;
  currentEtudiantId!: number;

  constructor(private https: HttpClient, private authService: AuthService, private dialog: MatDialog) {}

  ngOnInit() {
    this.currentEtudiantId = this.authService.getEtudiantId() ?? 0;
    this.getIdEquipeCurrent();
  }

  @ViewChild('invitationsTemplate') invitationsTemplate!: TemplateRef<any>;

  currentEquipeId: number | undefined;
  getIdEquipeCurrent() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Devoirs/${this.devoir?.id}/equipe`;

    this.https.get<number>(url, { withCredentials: true }).subscribe(
      (result) => {
        this.currentEquipeId = result;
        this.getEquipeDetailsMembres();
      }, (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  equipeDetailsMembres: EquipeDetailsMembres = { equipe: { id: 0, idDevoir: 0, etatSoumission: 0 }, etudiants: [] };
  getEquipeDetailsMembres() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Equipes/${this.currentEquipeId}/details-membres`;
    this.https.get<EquipeDetailsMembres>(url).subscribe(
      (result) => {
        this.equipeDetailsMembres = result;
        // 1. Current étudiant, 2. Accepté, 3. Invité, 4. Vide
        this.equipeDetailsMembres.etudiants.sort((a, b) => {
          if (a.etudiantId === this.currentEtudiantId) return -1;
          if (b.etudiantId === this.currentEtudiantId) return 1;
          if (a.accepte === b.accepte) return 0;
          return a.accepte ? -1 : 1;
        });
        this.getEtudiantsDisponibles();
      }, (error) => { console.error('Erreur:', error);}
    );
  }

  etudiantsDisponibles: Etudiant[] = [];
  getEtudiantsDisponibles() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Devoirs/${this.devoir?.id}/etudiants-disponibles`;

    this.https.get<Etudiant[]>(url).subscribe(
      (result) => {
        const membresEquipeActuels = new Set(
          this.equipeDetailsMembres.etudiants.map(member => member.etudiantId)
        );
        this.etudiantsDisponibles = result.filter(e => !membresEquipeActuels.has(parseInt(e.id)));
        this.getCurrentInvitations();
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  invitations: MembreInvitation[] = [];
  acceptedMembresInvitations: MembreInvitation[] = [];
  getCurrentInvitations() {
    if(this.equipeDetailsMembres.etudiants.length > 1)
      this.invitations = [];
    else {
      const host = window.location.hostname;
      let url = `https://${host}/api/Devoirs/${this.devoir?.id}/invitations`;

      this.https.get<MembreInvitation[]>(url, {withCredentials: true}).subscribe(
        (result) => {
          this.invitations = result;
          this.acceptedMembresInvitations = this.invitations.map(invitation => ({
            equipeId: invitation.equipeId,
            membres: invitation.membres.filter(m => m.accepte)
          }));
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
    }
  }

  getNombrePlacesVide() {
    let nombrePlacesVide = this.devoir.tailleMaxEquipes - this.equipeDetailsMembres.etudiants.length;

    return new Array(nombrePlacesVide);
  }

  /* INVITATIONS */

  voirInvitations() {
    this.openInvitationsDialog();
  }

  selectedEtudiantId: number | null = null;
  async inviterMembre(createur: boolean, idEtudiant: number) {
    await this.envoiInvitation(createur, idEtudiant);
  }

  async envoiInvitation(createur: boolean, idEtudiant: number): Promise<void> {
    if (!this.selectedEtudiantId) {
      this.openDialog("Veuillez selectionner un collègue de classe à inviter dans votre équipe.")
      return;
    }

    const payload = {
      idEtudiant: idEtudiant,
      idEquipe: this.currentEquipeId,
      accepte: createur
    };

    const host = window.location.hostname;
    let url = `https://${host}/api/Membres`;

    return new Promise((resolve, reject) => {
      this.https.post(url, payload).subscribe(
        () => {
          resolve();
          window.location.reload();
          },
        error => {
          console.error('Erreur:', error);
          reject(error);
        }
      );
    });
  }

  accepterInvitation(idEquipe: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Membres/invitation/${idEquipe}/accepter`;

    this.https.post(url, {}, {withCredentials: true}).subscribe(
      () => { window.location.reload(); }, (error) => { console.error('Erreur:', error); }
    );
  }

  refuserInvitation(idEquipe: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Membres/invitation/${idEquipe}/refuser`;

    this.https.post(url, {}, {withCredentials: true}).subscribe(
      () => {
        this.getCurrentInvitations();
        this.openInvitationsDialog();
      }, (error) => { console.error('Erreur:', error); }
    );
  }

  quitterEquipe() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Membres/${this.devoir?.id}/utilisateur-courant`;

    this.https.delete(url, {withCredentials: true}).subscribe(
      () => { window.location.reload(); }, (error) => { console.error('Erreur:', error); }
    );
  }

  openDialog(message: string) {
    this.dialog.open(DialogComponent, {
      data: { titre: "Erreur dans l'équipe", message },
    });
  }

  openInvitationsDialog() {
    this.dialog.open(DialogComponent, {
      data: { titre: "Invitations", template: this.invitationsTemplate },
    });
  }

  protected readonly EtatDevoir = EtatDevoir;
}
