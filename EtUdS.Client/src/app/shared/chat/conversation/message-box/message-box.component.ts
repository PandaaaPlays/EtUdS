import {Component, Input} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {NgClass, NgIf} from '@angular/common';

interface Vue {
  id: number;
  nom: string;
}

@Component({
  selector: 'message-box',
  standalone: true,
  templateUrl: './message-box.component.html',
  styleUrls: ['./message-box.component.css'],
  imports: [
    FormsModule,
    NgClass,
    NgIf
  ]
})

export class MessageBox {
  @Input() systemMessage!: boolean;
  @Input() envoiSelf!: boolean;
  @Input() nom!: string | null;
  @Input() date!: string;
  @Input() listeNomsVues!: Vue[] | null;
  @Input() message!: string;
  @Input() hiddenTitle!: boolean;

  titleVues: string = "";
  ngOnInit() {
    if(this.envoiSelf) {
      this.nom = "Vous";
    }
    if (this.listeNomsVues == null || this.listeNomsVues.length === 0) {
      this.titleVues = "";
    } else if (this.listeNomsVues.length === 1) {
      this.titleVues = `Vu par ${this.listeNomsVues[0].nom}`;
    } else if (this.listeNomsVues.length === 2) {
      this.titleVues = `Vu par ${this.listeNomsVues[0].nom} et ${this.listeNomsVues[1].nom}`;
    } else {
      const dernier = this.listeNomsVues.pop();
      const noms = this.listeNomsVues.map(vue => vue.nom);
      this.titleVues = `Vu par ${noms.join(", ")} et ${dernier?.nom}`;
    }
  }
}
