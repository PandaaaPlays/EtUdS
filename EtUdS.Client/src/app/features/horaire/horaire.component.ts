import { Component, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { Header } from '../../shared/header/header.component';
import { CommonModule } from '@angular/common';
import {HoraireEvent} from './horaire-event/horaire-event.component';
import {HttpClient} from '@angular/common/http';
import {formatDate, formatTime, getDate, getMonthAbbreviation, getStartAndEndOfWeek} from '../../utils/dates';
import {Router} from '@angular/router';

interface DevoirDetailCours {
  id: number;
  sigle: string;
  nom: string;
  dateLimiteSoumission: string;
}

interface SeanceDetailCours {
  idCours: number;
  sigle: string;
  titre: string;
  debutSeance: string;
  finSeance: string;
  local: string;
}

@Component({
  selector: 'horaire',
  standalone: true,
  templateUrl: './horaire.component.html',
  styleUrls: ['./horaire.component.css'],
  imports: [
    Header, HoraireEvent,
    CommonModule
  ]
})
export class Horaire implements AfterViewInit {
  public jours: { dayName: string, date: string, displayDate: string }[] = [];
  heures: string[] = Array.from({ length: 24 }, (_, i) =>
    `${i.toString().padStart(2, '0')}`
  );

  public devoirs: DevoirDetailCours[] = [];
  public seances: SeanceDetailCours[] = [];
  public today: string = '';
  private currentDate: Date = new Date();
  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit() {
    this.currentDate = new Date();
    this.today = formatDate(this.currentDate, false);
    this.updateWeek();
  }

  updateWeek() {
    this.jours = [];
    this.calculateDatesForWeek();
    this.getSeances();
    this.getDevoirs();
  }

  navigateToDevoir(id: number) {
    this.router.navigate(['/devoirs', id]);
  }

  navigateToCours(id: number) {
    this.router.navigate(['/cours', id]);
  }

  calculateDatesForWeek() {
    const daysInWeek = ["Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"];

    for (let i = 0; i < 7; i++) {
      const dayOffset = i - this.currentDate.getDay();
      const dayDate = new Date(this.currentDate);
      dayDate.setDate(this.currentDate.getDate() + dayOffset);

      const dayName = daysInWeek[dayDate.getDay()];
      const day = dayDate.getDate().toString().padStart(2, '0');
      const monthAbbreviation = getMonthAbbreviation(dayDate.getMonth());
      const formattedDate = `${day} ${monthAbbreviation}`;

      this.jours.push({ dayName, date: formatDate(dayDate, false), displayDate: formattedDate });
    }
  }

  getSeances() {
    const host = window.location.hostname;
    const { startDate, endDate } = getStartAndEndOfWeek(this.currentDate.toISOString().split('T')[0]);
    let url = `https://${host}/api/Seances/cours-details?StartDate=${startDate}&EndDate=${endDate}`;

    this.http.get<SeanceDetailCours[]>(url, {withCredentials: true})
      .subscribe(
        (result) => {
          this.seances = result;
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
  }

  getDevoirs() {
    const host = window.location.hostname;
    const { startDate, endDate } = getStartAndEndOfWeek(this.currentDate.toISOString().split('T')[0]);
    let url = `https://${host}/api/Devoirs/cours-details?StartDate=${startDate}&EndDate=${endDate}`;

    this.http.get<DevoirDetailCours[]>(url, {withCredentials: true})
      .subscribe(
        (result) => {
          this.devoirs = result;
        },
        (error) => {
          console.error('Erreur:', error);
        }
      );
  }

  // Scrolling de la page pour afficher 8h-17h par défaut.
  @ViewChild('horaireBody') horaireBody!: ElementRef;
  ngAfterViewInit() {
    if (this.horaireBody) {
      const horaireBodyElement = this.horaireBody.nativeElement;

      setTimeout(() => {
        const scrollHeight = horaireBodyElement.scrollHeight;
        const scrollPosition = scrollHeight * 0.334;

        horaireBodyElement.scrollTop = scrollPosition;
      }, 100);
    }
  }


  isMatchingSeanceHourAndDay(heure: string, dateString: string): SeanceDetailCours | null {
    for(const seance of this.seances) {
      const dateDebutSeance = getDate(seance.debutSeance.split('T')[0]);
      const heureDebutSeance = seance.debutSeance.split('T')[1].split(':')[0];
      const dateCase = getDate(dateString).getDate();
      if(dateDebutSeance.getDate() === dateCase && heureDebutSeance === heure)
        return seance;
    }
    return null;
  }

  isMatchingDevoirHourAndDay(heure: string, dateString: string): DevoirDetailCours | null {
    for(const devoir of this.devoirs) {
      const dateDevoir = getDate(devoir.dateLimiteSoumission.split('T')[0]);
      const heureDevoir = devoir.dateLimiteSoumission.split('T')[1].split(':')[0];
      const dateCase = getDate(dateString).getDate();

      if(dateDevoir.getDate() === dateCase && heureDevoir === heure)
        return devoir;
    }
    return null;
  }

  calculateEventTop(dateString: string, devoir: boolean): number {
    const date = new Date(dateString);
    if(devoir)
      return (date.getMinutes() / 60) * 100 - 56;
    else
      return (date.getMinutes() / 60) * 100 - 1;
  }

  calculateEventHeight(debut: string, fin: string): number {
    const dateDebut = new Date(debut);
    const dateFin = new Date(fin);

    const minutes = ((dateFin as any) - (dateDebut as any)) / (1000 * 60);

    let height = (minutes / 60) * 100;
    return height > 40 ? height : 40;
  }

  changeWeek(direction: 'gauche' | 'droite') {
    if (direction === 'gauche') {
      this.currentDate.setDate(this.currentDate.getDate() - 7);
    } else {
      this.currentDate.setDate(this.currentDate.getDate() + 7);
    }
    this.updateWeek();
  }

  protected readonly formatTime = formatTime;
}
