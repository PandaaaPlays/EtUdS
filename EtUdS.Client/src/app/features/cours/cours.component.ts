import { Component } from '@angular/core';
import {Header} from '../../shared/header/header.component';
import { HttpClient } from '@angular/common/http';
import { CoursCard } from './cours-card/cours-card.component';
import { CommonModule } from '@angular/common';

interface CoursDetails {
  id: number;
  idProfesseur: number;
  sigle: string;
  titre: string;
  prenomProf: string;
  nomProf: string;
}

interface ProfDetails {
  id: number;
  nom: string;
  prenom: string;
}

@Component({
  selector: 'cours',
  standalone: true,

  templateUrl: './cours.component.html',
  styleUrl: './cours.component.css',
  imports: [
    Header,
    CoursCard,
    CommonModule
  ]
})
export class Cours {
  public cours: CoursDetails[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getCours()
  }

  getCours() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Cours/utilisateur-courant`;

    this.http.get<CoursDetails[]>(url, {withCredentials: true})
      .subscribe(
        (result) => {
          this.cours = result;
          this.cours.forEach(cour => {
            console.log('Requesting Professeur with id:', cour.sigle);
            this.getProf(cour.idProfesseur).subscribe(
              (prof) => {
                cour.nomProf = prof.nom;
                cour.prenomProf = prof.prenom;
              },
              (error) => {
                console.error('Erreur:', error);
              }
            );
          });
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
  }

  getProf(id: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Professeur/${id}`;

    return this.http.get<ProfDetails>(url);
  }

}
