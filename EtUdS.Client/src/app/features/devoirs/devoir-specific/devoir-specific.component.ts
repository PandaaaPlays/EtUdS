import { Component } from '@angular/core';
import { Header } from '../../../shared/header/header.component';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {Router} from '@angular/router';
import {DevoirEquipe} from './devoir-equipe/devoir-equipe.component';
import {DevoirRemise} from './devoir-remise/devoir-remise.component';

interface Devoir {
  id: number,
  sigle: number,
  dateLimiteSoumission: string,
  nom: string,
  etat: number,
  note: number | null,
  tailleMaxEquipes: number
}

@Component({
  selector: 'devoirs',
  standalone: true,
  templateUrl: './devoir-specific.component.html',
  styleUrls: ['./devoir-specific.component.css'],
  imports: [
    Header,
    FormsModule,
    CommonModule,
    DevoirEquipe,
    DevoirRemise
  ]
})
export class DevoirSpecific {
  devoir!: Devoir;
  activeTab: string = localStorage.getItem('activeTab') || 'remise';
  idDevoir: string = "";

  setActiveTab(tab: string) {
    this.activeTab = tab;
    localStorage.setItem('activeTab', tab);
  }

  constructor(private router: Router, private http: HttpClient) {}

  ngOnInit() {
    this.activeTab = 'remise';
    this.idDevoir = this.router.url.split('/')[2]
    this.getDevoir(this.idDevoir);
  }

  getDevoir(id: string) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Devoirs/${id}/details`;

    this.http.get<Devoir>(url, {withCredentials: true}).subscribe(
      (result) => {
        this.devoir = result;
      },
      (error) => {
        console.error('Erreur:', error);
      }
    );
  }
}
