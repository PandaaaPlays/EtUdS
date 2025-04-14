import { Component } from '@angular/core';
import { DevoirCard } from './devoir-card/devoir-card.component';
import { Header } from '../../shared/header/header.component';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {AuthService} from '../../shared/auth/auth.service';

enum EtatDevoir {
  A_Faire = 1,
  En_Retard = 2,
  Soumis = 3,
  Corrige = 4
}

interface DevoirDetailCours {
  sigle: string;
  nom: string;
  etat: number;
  dateLimiteSoumission: string;
  note: number | null;
  id: number | null;
}

interface Cours {
  sigle: string;
}

@Component({
  selector: 'devoirs',
  standalone: true,
  templateUrl: './devoirs.component.html',
  styleUrls: ['./devoirs.component.css'],
  imports: [
    DevoirCard,
    Header,
    FormsModule,
    CommonModule
  ]
})
export class Devoirs {
  public devoirs: DevoirDetailCours[] = [];
  public cours: Cours[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getDevoirs();
    this.getCours();
  }

  selectedFilter: string = '';

  getDevoirs() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Devoirs/cours-details`;

    if (this.selectedFilter) {
      const [group, value] = this.selectedFilter.split(':');
      url += `?${group}=${encodeURIComponent(value)}`;
    }

    this.http.get<DevoirDetailCours[]>(url, {withCredentials: true}).subscribe(
      (result) => {
        this.devoirs = result.sort((a, b) => {
          if (a.etat !== b.etat) {
            return (
              this.getEtatPriority(a.etat) - this.getEtatPriority(b.etat)
            );
          }
          return new Date(a.dateLimiteSoumission).getTime() -
            new Date(b.dateLimiteSoumission).getTime();
        });
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }

  getCours() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Cours/utilisateur-courant`;

    this.http.get<Cours[]>(url, {withCredentials: true})
      .subscribe(
        (result) => {
          this.cours = result.filter((value, index, self) =>
            index === self.findIndex((c) => c.sigle === value.sigle)
          );
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
  }

  getEtatLabel(etat: number): string {
    switch (etat) {
      case EtatDevoir.A_Faire:
        return 'À faire';
      case EtatDevoir.En_Retard:
        return 'En retard';
      case EtatDevoir.Soumis:
        return 'Soumis';
      case EtatDevoir.Corrige:
        return 'Évalué';
      default:
        return 'Inconnu';
    }
  }

  getEtatPriority(etat: number): number {
    switch (etat) {
      case EtatDevoir.En_Retard:
        return 0;
      case EtatDevoir.A_Faire:
        return 1;
      case EtatDevoir.Corrige:
        return 2;
      case EtatDevoir.Soumis:
        return 3;
      default:
        return 4;
    }
  }
}
