import { Component } from '@angular/core';
import {DialogComponent} from '../../shared/dialog/dialog.component';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import {ButtonEtuds} from "../../shared/button-etuds/button-etuds.component";

interface Etudiant {
  id: number;
  nom: string;
  prenom: string;
  courriel: string;
  motDePasse: string;
}

@Component({
    selector: 'app-inscription',
    standalone: true,

    templateUrl: './inscription.component.html',
    imports: [
        ButtonEtuds
    ],
    styleUrl: './inscription.component.css'
})
export class Inscription {

  constructor(private http: HttpClient, private dialog: MatDialog) {
  }

  returnToConnection(){
    window.location.href = "/connexion";
  }

  inscrireUtilisateur(){
    const courrielInput = document.getElementById("courriel") as HTMLInputElement;
    const prenomInput = document.getElementById("prenom") as HTMLInputElement;
    const nomInput = document.getElementById("nom") as HTMLInputElement;
    const MDPInput = document.getElementById("MDP") as HTMLInputElement;
    const MDPConfirmeInput = document.getElementById("MDPConfirme") as HTMLInputElement;

    const courriel = courrielInput.value;
    const prenom = prenomInput.value;
    const nom = nomInput.value;
    const motDePasse = MDPInput.value;
    const motDePasseConfirme = MDPConfirmeInput.value;

    const host = window.location.hostname;
    let url = `https://${host}/api/Inscription`;

    if (!courriel || !prenom || !nom || !motDePasse || !motDePasseConfirme) {
      this.openDialog(false, "Tous les champs doivent être remplis.");
      return;
    }

    if(motDePasse !== motDePasseConfirme){
      this.openDialog(false, "Les mots de passes ne sont pas identiques.");
      MDPConfirmeInput.value = "";
      return;
    }

    this.http.post<Etudiant>(url, {courriel, prenom, nom, motDePasse})
      .subscribe(
        (result) => {
          const dialogRef= this.openDialog(true, "Votre compte a été créé, veuillez vous connecter.");
          dialogRef.afterClosed().subscribe(() => {
            window.location.href = "/connexion";
          });
        },
        error => {
          if (error.status === 409) {
            this.openDialog(false, "Un compte avec ce courriel existe déjà.");
          } else {
            this.openDialog(false, "Une erreur s'est produite, veuillez réessayer plus tard.");
            console.error('Erreur:', error);
          }
        }
      );
  }

  openDialog(succes: boolean, message: string){
    if(!succes)
      return this.dialog.open(DialogComponent, {
        data: { titre: "Erreur lors de l'inscription", message },
      });
    else
      return this.dialog.open(DialogComponent, {
        data: { couleurBouton: 'bleu', titre: "Inscription réussie", message },
      });
  }
}
