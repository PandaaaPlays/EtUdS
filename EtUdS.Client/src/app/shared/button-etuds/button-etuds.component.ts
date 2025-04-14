import {Component, Input} from '@angular/core';
import {NgClass} from '@angular/common';


@Component({
  selector: 'button-etuds',
  standalone: true,
  templateUrl: './button-etuds.component.html',
  styleUrls: ['./button-etuds.component.css'],
  imports: [
    NgClass
  ]
})

export class ButtonEtuds {
  @Input() couleur!: string;
  @Input() texte!: string;
  @Input() disabled!: boolean;
  @Input() fullWidth: boolean = false;
}
