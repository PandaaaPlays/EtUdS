import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { EtUdSNavbar } from './shared/etuds-navbar/etuds-navbar.component';
import { Cours } from './features/cours/cours.component';
import { Devoirs } from './features/devoirs/devoirs.component';
import { Horaire } from './features/horaire/horaire.component';
import { DevoirCard } from './features/devoirs/devoir-card/devoir-card.component';
import { Header } from './shared/header/header.component';
import { Connexion } from './features/connexion/connexion.component';
import { Inscription } from './features/inscription/inscription.component';
import { DevoirSpecific } from './features/devoirs/devoir-specific/devoir-specific.component';
import { DevoirEquipe } from './features/devoirs/devoir-specific/devoir-equipe/devoir-equipe.component';
import { DialogComponent } from './shared/dialog/dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CoursCard } from './features/cours/cours-card/cours-card.component';
import { CoursSpecific } from './features/cours/cours-specific/cours-specific.component';
import { CoursNote } from './features/cours/cours-specific/cours-note/cours-note.component';
import { CoursDevoir } from './features/cours/cours-specific/cours-devoir/cours-devoir.component';
import {Chat} from "./shared/chat/chat.component";

@NgModule({
  declarations: [
    AppComponent,

  ],
    imports: [
        BrowserModule, HttpClientModule, MatIconModule,
        AppRoutingModule, EtUdSNavbar, Header,
        Devoirs, DevoirSpecific, Cours, Horaire, DevoirCard, CoursCard,
        DevoirEquipe,
        CoursSpecific,
        CoursNote,
        CoursDevoir,
        Connexion, Inscription, DialogComponent, MatDialogModule, MatButtonModule, Chat,
    ],
  providers: [],
  exports: [MatDialogModule, MatButtonModule, MatIconModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
