import {Component, Input} from '@angular/core';
import {NgForOf, NgIf, NgStyle} from '@angular/common';

@Component({
  selector: 'header',
  standalone: true,

  templateUrl: './header.component.html',
  imports: [
    NgForOf,
    NgIf,
    NgStyle
  ],
  styleUrl: './header.component.css'
})
export class Header {
  @Input() titres: { text: string; lien: string }[] | undefined;
  @Input() textSize: number = 45;
}
