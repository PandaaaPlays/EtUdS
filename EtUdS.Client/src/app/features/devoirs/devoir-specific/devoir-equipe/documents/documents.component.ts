import {Component, ElementRef, Input, OnDestroy, ViewChild} from '@angular/core';
import {Header} from '../../../../../shared/header/header.component';
import {HttpClient} from '@angular/common/http';
import {NgForOf, NgIf} from '@angular/common';
import {AuthService} from '../../../../../shared/auth/auth.service';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../../../../../shared/dialog/dialog.component';
import {DragBox} from '../../../../../shared/drag-box/drag-box.component';

interface Document {
  id: number,
  nomFichier: string,
  idEtudiant: number,
  prenom: string,
  nom: string,
  taille: number
}

@Component({
  selector: 'documents',
  standalone: true,

  templateUrl: './documents.component.html',
  imports: [
    Header,
    NgForOf,
    NgIf,
    DragBox
  ],
  styleUrl: './documents.component.css'
})
export class Documents implements OnDestroy {
  @Input() idDevoir!: number;
  currentEtudiantId!: number;
  private refreshInterval!: any;

  constructor(private http: HttpClient, private authService: AuthService, private dialog: MatDialog) {}

  ngOnInit() {
    this.getFichiers();
    this.currentEtudiantId = this.authService.getEtudiantId() ?? 0;
    this.refreshInterval = setInterval(() => {
      this.getFichiers();
    }, 15000);
  }

  ngOnDestroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  fichiers: Document[] = [];
  getFichiers() {
    const host = window.location.hostname;
    let url = `https://${host}/api/Documents/${this.idDevoir}/equipe`;
    this.http.get<Document[]>(url, {withCredentials: true}).subscribe(
      (response) => { this.fichiers = response },
      (error) => { this.openDialog("Les fichiers n'ont pas pu être récuperés correctement.") }
    );
  }

  downloadFile(idFichier: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Documents/${idFichier}`;

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
        this.getFichiers();
      }
    );
  }


  deleteFile(id: number) {
    const host = window.location.hostname;
    let url = `https://${host}/api/Documents/${id}`;

    this.http.delete(url, { withCredentials: true }).subscribe(
      () => {
        this.fichiers = this.fichiers.filter(fichier => fichier.id !== id);
      },
      (error) => { this.openDialog("Le fichier n'a pas pu être supprimé correctement.") }
    );
  }

  uploadFiles(files: FileList) {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      formData.append('fichiers', files[i]);
    }

    const host = window.location.hostname;
    let url = `https://${host}/api/Documents/${this.idDevoir}`;
    this.http.post(url, formData, {withCredentials: true}).subscribe(
      (response) => { this.getFichiers() },
      (error) => { this.openDialog("Le ou les fichier(s) n'ont pas été envoyé correctement. La taille est-elle trop grande?") }
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
}
