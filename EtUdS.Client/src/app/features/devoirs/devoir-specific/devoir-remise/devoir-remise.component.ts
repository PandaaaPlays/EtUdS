import {Component, Input, TemplateRef, ViewChild} from '@angular/core';
import {Header} from '../../../../shared/header/header.component';
import {FormsModule} from '@angular/forms';
import {formatDate} from '../../../../utils/dates';
import {NgForOf, NgIf} from '@angular/common';
import {Soumissions} from './soumissions/soumissions.component';
import {DialogComponent} from '../../../../shared/dialog/dialog.component';
import {HttpClient} from '@angular/common/http';
import {MatDialog} from '@angular/material/dialog';

interface Devoir {
  id: number,
  sigle: number,
  dateLimiteSoumission: string,
  nom: string,
  etat: number,
  note: number | null,
  tailleMaxEquipes: number
}

interface Soumission {
  id: number,
  nomFichier: string,
  nomFichierCorrection: string | null
  tempsSoumission: string,
  taille: number
}

@Component({
  selector: 'devoir-remise',
  standalone: true,

  templateUrl: './devoir-remise.component.html',
  imports: [
    Header,
    FormsModule,
    NgIf,
    Soumissions,
    NgForOf
  ],
  styleUrl: './devoir-remise.component.css'
})
export class DevoirRemise  {
  @Input() devoir!: Devoir;

  constructor(private https: HttpClient, private dialog: MatDialog) {}

  @ViewChild('correctionsTemplate') invitationsTemplate!: TemplateRef<any>;

  getNotePercentage(): number {
    return this.devoir.note !== null ? (this.devoir.note / 100) * 100 : 0;
  }

  getEtat(etat: number): string {
    switch (etat) {
      case 1:
        return "À faire";
      case 2:
        return "En retard";
      case 3:
        return "Soumis";
      case 4:
        return "Évalué";
      default:
        return "Inconnu";
    }
  }

  soumissionsCorriges: Soumission[] = [];
  handleSoumissionsCorriges(soumissionsCorriges: Soumission[]) {
    this.soumissionsCorriges = soumissionsCorriges;
  }

  voirCorrections() {
    this.openCorrectionsDialog();
  }

  openCorrectionsDialog() {
    this.dialog.open(DialogComponent, {
      data: { titre: "Corrections", template: this.invitationsTemplate },
    });
  }

  downloadFile(idFichier: number) {
    const host = window.location.hostname;

    let url = `https://${host}/api/Soumissions/${idFichier}/Correction`;

    this.https.get(url, { responseType: 'blob', withCredentials: true, observe: 'response' }).subscribe(
      (response) => {
        const fileName = response.headers.get('X-File-Name');
        if(fileName) {
          const blob = response.body ? new Blob([response.body], {type: 'application/octet-stream'}) : new Blob();

          const a = document.createElement('a');
          a.href = URL.createObjectURL(blob);
          a.download = fileName;
          a.click();
          URL.revokeObjectURL(a.href);
        }
      },
      (error) => {
        this.openDialog("Le fichier n'a pas pu être téléchargé.")
      }
    );
  }

  openDialog(message: string) {
    this.dialog.open(DialogComponent, {
      data: { titre: "Erreur dans la remise", message },
    });
  }

  protected readonly formatDate = formatDate;
}
