import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'conversation-button',
  standalone: true,
  templateUrl: './conversation-button.component.html',
  styleUrls: ['./conversation-button.component.css'],
  imports: [
    NgIf
  ]
})

export class ConversationButton {
  @Input() nom: string = "";
  @Input() prenom: string | null = null;
  @Input() message: string | null = null;
  @Input() date: string | null = null;
  @Input() hasNotification: boolean = false;

  hasLastMessage(): boolean {
    return this.prenom != null && this.message != null && this.date != null;
  }
}
