import {Component, Inject, Input} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import {NgIf, NgStyle, NgTemplateOutlet} from '@angular/common';
import {ButtonEtuds} from '../button-etuds/button-etuds.component';

@Component({
  selector: 'app-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, NgTemplateOutlet, NgIf, NgStyle, ButtonEtuds],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css'
})
export class DialogComponent {
  private readonly action: Function | null = null;

  constructor(
    public dialogRef: MatDialogRef<DialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      template: any,
      titre: string,
      message: string,
      width: number,
      couleurBouton: string,
      texteBouton: string,
      action: Function
    }
  ) {
    if(data.width == null) {
      data.width = 400;
    }
    if(data.couleurBouton == null) {
      data.couleurBouton = 'rouge';
    }
    if(data.texteBouton == null) {
      data.texteBouton = 'Fermer';
    }
    if(data.action != null) {
      this.action = data.action;
    }
  }

  close(): void {
    this.dialogRef.close();
  }

  closeWithAction(): void {
    if(this.action !== null)
      this.action();
    this.close();
  }
}
