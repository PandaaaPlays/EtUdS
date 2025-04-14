import { Component } from '@angular/core';
import { Header } from '../../../shared/header/header.component';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {Router} from '@angular/router';
import {CoursNote} from './cours-note/cours-note.component';
import {CoursDevoir} from './cours-devoir/cours-devoir.component';


interface Cours {
  id: number;
  idProfesseur: number;
  sigle: string;
  titre: string;
  prenomProf: string;
  nomProf: string;
  horaire: Seance[];
}

interface Seance {
  id: number;
  idCours: number;
  dateDebut: string;
  dateFin: string;
  local: string;
  isLabo: boolean;
}

@Component({
  selector: 'cours-specific',
  standalone: true,
  templateUrl: './cours-specific.component.html',
  styleUrl: './cours-specific.component.css',
  imports: [
    Header,
    FormsModule,
    CommonModule,
    CoursNote,
    CoursDevoir,
  ]
})
export class CoursSpecific {
  cours!: Cours;
  activeTab: string = localStorage.getItem('activeTab') || 'note';
  idCours: string = "";

  setActiveTab(tab: string) {
    this.activeTab = tab;
    localStorage.setItem('activeTab', tab);
  }

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit() {
    this.activeTab = 'note';
    this.idCours = this.router.url.split('/')[2]
    this.getCours(this.idCours);
  }

  getCours(id: string) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Cours/${id}`;

    this.http.get<Cours>(url, {withCredentials: true}).subscribe(
      (result) => {
        this.cours = result;
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }
}
