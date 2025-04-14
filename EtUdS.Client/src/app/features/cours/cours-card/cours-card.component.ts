import { Component, Input } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'cours-card',
  standalone: true,

  templateUrl: './cours-card.component.html',
  styleUrl: './cours-card.component.css'
})

export class CoursCard {
  @Input() id: number | null = null;
  @Input() sigle: string = 'Sigle';
  @Input() titre: string = 'Titre';
  @Input() nomProf: string = 'NomProf';
  @Input() prenomProf: string = 'PrenomProf';

  constructor(private router: Router) {}

  navigateToDevoir() {
    this.router.navigate(['/cours', this.id]);
  }
}
