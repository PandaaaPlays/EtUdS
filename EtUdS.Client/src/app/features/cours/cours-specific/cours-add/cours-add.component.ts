import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {DialogComponent} from '../../../../shared/dialog/dialog.component';
import {Router} from '@angular/router';
import {DragBox} from '../../../../shared/drag-box/drag-box.component';


@Component({
  selector: 'cours-add',
  standalone: true,
  templateUrl: './cours-add.component.html',
  styleUrl: './cours-add.component.css',
  imports: [
    FormsModule,
    CommonModule,
    DragBox
  ]
})

// Cette classe est utilisé seulement pour simplifier la création de cours...
// Elle ne doit pas être évalué et est inaccessible dans le site (serait normalement supprimé!)
export class CoursAdd {

  constructor(private router: Router, private http: HttpClient, private dialog: MatDialog) {}

  uploadFiles(files: FileList) {

    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      formData.append('fichiers', files[i]);
    }

    const idCours = this.router.url.split('/')[2]
    const host = window.location.hostname;
    let url = `https://${host}/api/NoteDeCours/${idCours}`;
    this.http.post(url, formData, {withCredentials: true}).subscribe(
      (response) => {  },
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
