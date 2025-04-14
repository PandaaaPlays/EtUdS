import {Component, OnInit} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import {DialogComponent} from '../../shared/dialog/dialog.component';
import {AuthService} from '../../shared/auth/auth.service';
import {Router} from '@angular/router';
import {ButtonEtuds} from '../../shared/button-etuds/button-etuds.component';
interface Etudiant {
  id: number;
  nom: string;
  prenom: string;
  courriel: string;
  motDePasse: string;
}


@Component({
  selector: 'connexion',
  standalone: true,
  templateUrl: './connexion.component.html',
  imports: [
    ButtonEtuds
  ],
  styleUrl: './connexion.component.css'
})
export class Connexion implements OnInit {
  public user: Etudiant | null = null;

  constructor(private https: HttpClient,
              private dialog: MatDialog,
              private authService: AuthService,
              private router: Router) {}

  ngOnInit() {
    this.authService.logout();
  }

  authenticateUser(){
    const courrielInput = document.getElementById("email") as HTMLInputElement;
    const MDPInput = document.getElementById("password") as HTMLInputElement;

    const courriel = courrielInput.value;
    const motDePasse = MDPInput.value;

    const host = window.location.hostname;
    let url = `https://${host}/api/Connexion`;

    if(courriel == "" || motDePasse == ""){
      this.openDialog("Veuillez remplir le formulaire au complet.");
      courrielInput.value = "";
      MDPInput.value = "";
    }
    else{
      this.https.post<Etudiant>(url, {courriel, motDePasse}, { withCredentials: true })
        .subscribe(
          (result) => {
            this.user = result;
            this.authService.getEtudiant().subscribe(
              (user) => {
                if (user) {
                  this.router.navigate(['/cours']);
                } else {
                  this.openDialog("Erreur de connexion. Veuillez réessayer.");
                }
              },
              (error) => {
                this.openDialog("Erreur de connexion. Veuillez réessayer.");
                console.error('Erreur:', error);
              }
            );
          },
          (error) => {
            if(error.status === 401) {
              this.openDialog("Courriel ou mot de passe invalide.");
            }
            else{
              this.openDialog("Une erreur s'est produite, veuillez réessayer plus tard.");
              console.error('Erreur:', error);
            }
            MDPInput.value = "";
          }
        );
    }
  }

  openDialog( message: string){
    this.dialog.open(DialogComponent, {
      data: { titre: "Erreur lors de la connexion", message },
    });
  }

}
