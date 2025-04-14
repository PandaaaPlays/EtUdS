import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Cours } from './features/cours/cours.component';
import { Devoirs } from './features/devoirs/devoirs.component';
import { Horaire } from './features/horaire/horaire.component';
import {Connexion} from './features/connexion/connexion.component';
import {Inscription} from './features/inscription/inscription.component';
import {DevoirSpecific} from './features/devoirs/devoir-specific/devoir-specific.component';
import {CoursSpecific} from './features/cours/cours-specific/cours-specific.component';
import {AuthService} from './shared/auth/auth.service';
import {CoursAdd} from './features/cours/cours-specific/cours-add/cours-add.component';

const routes: Routes = [
  { path: 'cours', component: Cours, canActivate: [AuthService] },
  { path: 'devoirs', component: Devoirs, canActivate: [AuthService] },
  { path: 'horaire', component: Horaire, canActivate: [AuthService] },
  { path: 'connexion', component: Connexion },
  { path: 'inscription', component: Inscription },
  { path: 'devoirs/:code', component: DevoirSpecific, canActivate: [AuthService] },
  { path: 'cours/:code', component: CoursSpecific, canActivate: [AuthService] },
  { path: 'cours/:code/add', component: CoursAdd, canActivate: [AuthService] },
  { path: '', redirectTo: '/connexion', pathMatch: 'full' }  // Default route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
