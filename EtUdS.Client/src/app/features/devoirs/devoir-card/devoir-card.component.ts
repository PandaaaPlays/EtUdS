import {Component, Input} from '@angular/core';
import {DatePipe, NgStyle} from '@angular/common';
import {formatDate} from '../../../utils/dates';
import {Router} from '@angular/router';

@Component({
  selector: 'devoir-card',
  standalone: true,

  templateUrl: './devoir-card.component.html',
  imports: [
    NgStyle
  ],
  styleUrl: './devoir-card.component.css'
})
export class DevoirCard {
  @Input() id: number | null = null;
  @Input() code: string = 'Code';
  @Input() nom: string = 'Nom';
  @Input() etat: string = 'N/A';
  @Input() date: string = 'N/A';
  @Input() note: number | null = null;

  tempsRestant: string = ''
  headerColor: string = '';
  private intervalId: any;

  ngOnInit(): void {
    this.updateHeaderColor();
    this.updateTempsRestant();
    this.intervalId = setInterval(() => this.updateTempsRestant(), 1000);
  }

  constructor(private router: Router) {}

  navigateToDevoir() {
    this.router.navigate(['/devoirs', this.id]);
  }

  private updateHeaderColor(): void {
    const dueDate = new Date(this.date);
    const now = new Date();
    const diffTime = (dueDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24);

    if (this.etat === 'Évalué') {
      this.headerColor = 'rgba(59, 200, 216, 1)';
    } else if (this.etat === 'Soumis') {
      this.headerColor = '#999999';
    } else if (dueDate < now) {
      this.headerColor = '#424242';
    } else if (diffTime <= 1) {
      this.headerColor = '#B80000';
    } else if (diffTime <= 3) {
      this.headerColor = '#E7AC00';
    } else {
      this.headerColor = '#00B486';
    }
  }

  private updateTempsRestant(): void {
    const dueDate = new Date(this.date);
    const now = new Date();
    const diff = dueDate.getTime() - now.getTime();

    if (diff <= 0) {
      this.tempsRestant = 'Temps écoulé...';
    } else {
      const seconds = Math.floor((diff / 1000) % 60);
      const minutes = Math.floor((diff / (1000 * 60)) % 60);
      const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
      const days = Math.floor(diff / (1000 * 60 * 60 * 24));

      const parts: string[] = [];
      if (days > 0) parts.push(`${days}j`);
      if (hours > 0) parts.push(`${hours}h`);
      if (minutes > 0) parts.push(`${minutes}m`);
      if (seconds > 0) parts.push(`${seconds}s`);

      this.tempsRestant = parts.join(' ');
    }
  }

  getFormattedNote(note: number | null): string {
    return note === null ? 'Non disponible' : note.toString() + ' %';
  }

  protected readonly formatDate = formatDate;
}
