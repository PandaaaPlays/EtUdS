import { Component, Input } from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../../../../shared/dialog/dialog.component';
import {HttpClient} from '@angular/common/http';
import {CommonModule} from '@angular/common';
import {MatIcon} from '@angular/material/icon';
import {getMonthAbbreviation} from '../../../../utils/dates';
import {CoursAdd} from '../cours-add/cours-add.component';

interface noteDeCours {
  id: number,
  idCours: number,
  date: string,
  nomFichier: string,
  semaine: number;
  taille: number;
}

@Component({
  selector: 'cours-note',
  standalone: true,
  imports: [
    CommonModule,
    MatIcon,
    CoursAdd,
  ],
  templateUrl: './cours-note.component.html',
  styleUrl: './cours-note.component.css'
})
export class CoursNote {
  @Input() idCours!: number;
  private refreshInterval!: any;
  fichiers: noteDeCours[] = [];
  fichiersParSemaine: { semaine: number; fichiers: noteDeCours[] }[] = [];
  semaines: { semaine: number; dateDebut: Date; dateFin: Date }[] = [];

  constructor(private http: HttpClient, private dialog: MatDialog) {}

  ngOnInit() {
    this.calculateSemainesDates();
    this.getFichiers();
    this.refreshInterval = setInterval(() => {
      this.getFichiers();
    }, 15000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  calculateSemainesDates() {
    const SEMESTER_START_DATE = new Date('2025-01-13');
    const WEEKS_IN_SEMESTER = 15;

    this.semaines = Array.from({ length: WEEKS_IN_SEMESTER }, (_, i) => {
      const start = new Date(SEMESTER_START_DATE);
      start.setDate(start.getDate() + i * 7);

      const end = new Date(start);
      end.setDate(end.getDate() + 6);

      return {
        semaine: i + 1,
        dateDebut: start,
        dateFin: end,
      };
    });
  }

  getFichiers() {
    const host = window.location.hostname;
    let url = `https://${host}/api/NoteDeCours/${this.idCours}`;
    const SEMESTER_START_DATE = new Date('2025-01-12');

    this.http.get<noteDeCours[]>(url, { withCredentials: true }).subscribe(
      (response) => {
        this.fichiers = response.map((fichier) => {
          const fileDate = new Date(fichier.date);
          const semaine = Math.floor(
            (fileDate.getTime() - SEMESTER_START_DATE.getTime()) / (7 * 24 * 60 * 60 * 1000)
          ) + 1;
          return { ...fichier, semaine };
        });
        this.groupFichiersSemaine();
      },
      (error) => {
        this.openDialog("Les fichiers n'ont pas pu être récupérés correctement.");
      }
    );
  }

  groupFichiersSemaine() {
    const grouped = this.fichiers.reduce((acc, fichier) => {
      acc[fichier.semaine] = acc[fichier.semaine] || [];
      acc[fichier.semaine].push(fichier);
      return acc;
    }, {} as Record<number, noteDeCours[]>);

    this.fichiersParSemaine = Object.entries(grouped).map(([semaine, fichiers]) => ({
      semaine: +semaine,
      fichiers,
    }));

    this.collapsedSemaines = new Set(
      this.semaines
        .filter(
          (semaine) =>
            !this.fichiersParSemaine.some((item) => item.semaine === semaine.semaine && item.fichiers.length > 0)
        )
        .map((semaine) => semaine.semaine)
    );
  }

  getFichiersSemaine(semaine: number): noteDeCours[] {
    const data = this.fichiersParSemaine.find((item) => item.semaine === semaine);
    return data ? data.fichiers : [];
  }

  telechargerFicher(id: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/NoteDeCours/${this.idCours}/${id}`;

    this.http.get(url, { responseType: 'blob', withCredentials: true, observe: 'response' }).subscribe(
      (response) => {
        const fileName = response.headers.get('X-File-Name');
        if (fileName) {
          const blob = response.body ? new Blob([response.body], { type: 'application/octet-stream' }) : new Blob();

          const a = document.createElement('a');
          a.href = URL.createObjectURL(blob);
          a.download = fileName;
          a.click();
          URL.revokeObjectURL(a.href);
        }
      },
      (error) => {
        this.openDialog("Le fichier n'a pas pu être téléchargé.");
      }
    );
  }

  collapsedSemaines: Set<number> = new Set();
  toggleCollapse(semaine: number): void {
    if (this.collapsedSemaines.has(semaine)) {
      this.collapsedSemaines.delete(semaine);
    } else {
      this.collapsedSemaines.add(semaine);
    }
  }

  openDialog(message: string) {
    if (this.dialog.openDialogs.length > 0) {
      return;
    }

    this.dialog.open(DialogComponent, {
      data: { titre: "Une erreur est survenue", message },
    });
  }

  formatDate = (date: Date) => {
    let day = date.getDate();
    let mois = getMonthAbbreviation(date.getMonth());

    return day + " " + mois;
  };

  formatDateString = (dateString: string) => {
    let date = new Date(dateString);
    let day = date.getDate();
    let mois = getMonthAbbreviation(date.getMonth());

    return day + " " + mois;
  };
}
