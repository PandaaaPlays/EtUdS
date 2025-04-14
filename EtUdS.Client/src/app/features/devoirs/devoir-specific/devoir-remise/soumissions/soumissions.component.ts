import {Component, EventEmitter, Input, Output} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Header} from '../../../../../shared/header/header.component';
import {DragBox} from '../../../../../shared/drag-box/drag-box.component';
import {HttpClient} from '@angular/common/http';
import {AuthService} from '../../../../../shared/auth/auth.service';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../../../../../shared/dialog/dialog.component';
import {NgForOf, NgIf} from '@angular/common';
import {formatDate} from '../../../../../utils/dates';
import {ButtonEtuds} from '../../../../../shared/button-etuds/button-etuds.component';

interface Soumission {
  id: number,
  nomFichier: string,
  nomFichierCorrection: string | null
  tempsSoumission: string,
  taille: number
}

@Component({
  selector: 'soumissions',
  standalone: true,

  templateUrl: './soumissions.component.html',
  imports: [
    Header,
    FormsModule,
    DragBox,
    NgForOf,
    NgIf,
    ButtonEtuds
  ],
  styleUrl: './soumissions.component.css'
})
export class Soumissions  {
  @Input() idDevoir!: number;
  @Output() soumissionsCorrigesChange = new EventEmitter<Soumission[]>();

  currentEtudiantId!: number;
  private refreshInterval!: any;
  private formData: FormData | null = null;
  protected nbFichier: number = 0;

  constructor(private http: HttpClient, private authService: AuthService, private dialog: MatDialog) {}

  ngOnInit() {
    this.getSoumissions();
    this.currentEtudiantId = this.authService.getEtudiantId() ?? 0;
    this.refreshInterval = setInterval(() => {
      this.getSoumissions();
    }, 15000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  ajouterFichiers(files: FileList) {
    this.formData = new FormData();
    this.nbFichier = 0;
    for (let i = 0; i < files.length; i++) {
      this.formData.append('fichiers', files[i]);
      this.nbFichier++;
    }
  }

  annuler() {
    this.nbFichier = 0;
    this.formData = null;
  }

  soumettre() {
    if(this.nbFichier == 0) {
      this.openDialog("Veuillez choisir au moins un fichier à soumettre.")
      return;
    }
    const host = window.location.hostname;
    let url = `https://${host}/api/Soumissions/${this.idDevoir}`;
    this.http.post(url, this.formData, {withCredentials: true}).subscribe(
      (response) => { window.location.reload(); },
      (error) => { this.openDialog("Le ou les fichier(s) n'ont pas été envoyé correctement. La taille est-elle trop grande?") }
    );
    this.nbFichier = 0;
    this.formData = null;
  }

  soumissions: Soumission[] = [];
  getSoumissions() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Soumissions/${this.idDevoir}/equipe`;
    this.http.get<Soumission[]>(url, {withCredentials: true}).subscribe(
      (response) => {
        this.soumissions = response
        let soumissionsCorriges = this.soumissions.filter(soumission => soumission.nomFichierCorrection !== null);
        this.soumissionsCorrigesChange.emit(soumissionsCorriges);
      },
      (error) => { this.openDialog("Les soumissions n'ont pas pu être récuperés correctement.") }
    );
  }

  downloadFile(idFichier: number) {
    const host = window.location.hostname;

    let url = `https://${host}/api/Soumissions/${idFichier}`;

    this.http.get(url, { responseType: 'blob', withCredentials: true, observe: 'response' }).subscribe(
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
        this.getSoumissions();
      }
    );
  }

  deleteFile(id: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Soumissions/${id}`;

    this.http.delete(url, { withCredentials: true }).subscribe(
      () => {
        window.location.reload();
      },
      (error) => { this.openDialog("Le fichier n'a pas pu être supprimé correctement.") }
    );
  }

  openDialog( message: string){
    if (this.dialog.openDialogs.length > 0) {
      return;
    }

    this.dialog.open(DialogComponent, {
      data: { titre: "Une erreur est survenue", message },
    });
  }

  protected readonly formatDate = formatDate;
}
