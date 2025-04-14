import {ChangeDetectorRef, Component, ElementRef, Input, QueryList, ViewChildren} from '@angular/core';
import {NgIf} from '@angular/common';
import {HttpClient} from '@angular/common/http';

export interface MembreEtudiant {
  membreId: number;
  accepte: boolean;
  etudiantId: number;
  prenom: string;
  nom: string;
  courriel: string;
}

@Component({
  selector: 'membre',
  standalone: true,

  templateUrl: './membre.component.html',
  imports: [
    NgIf
  ],
  styleUrl: './membre.component.css'
})
export class Membre {
  @Input() etudiant!: MembreEtudiant;
  @Input() vide: boolean = false;
  @Input() kickable: boolean = false;

  constructor(private http: HttpClient) {}

  kick() {

    const host = window.location.hostname;
    let url = `https://${host}/api/Membres/${this.etudiant.membreId}`;

    this.http.delete(url).subscribe(
      () => { window.location.reload(); }, (error) => { console.error('Erreur:', error); }
    );
  }
}
