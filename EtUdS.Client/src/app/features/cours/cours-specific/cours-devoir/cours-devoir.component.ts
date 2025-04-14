import {Component, Input} from '@angular/core';
import {DevoirCard} from '../../../devoirs/devoir-card/devoir-card.component';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

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


@Component({
  selector: 'cours-devoir',
  standalone: true,

  templateUrl: './cours-devoir.component.html',
  imports: [
    DevoirCard,
    FormsModule,
    CommonModule
  ],
  styleUrl: './cours-devoir.component.css'
})
export class CoursDevoir{
  @Input() sigleCours: string = 'SigleCours';
  public devoirs: DevoirDetailCours[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getDevoirs();
  }

  selectedFilter: string = '';

  getDevoirs() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Devoirs/cours-details?cours=${this.sigleCours}`;

    if (this.selectedFilter) {
      const [group, value] = this.selectedFilter.split(':');
      url += `&${group}=${encodeURIComponent(value)}`;
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
