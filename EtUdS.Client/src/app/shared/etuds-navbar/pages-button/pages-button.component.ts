import {Component, Input, OnInit} from '@angular/core';
import {NgClass, NgIf} from '@angular/common';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {filter} from 'rxjs';
import {C} from '@angular/cdk/keycodes';

@Component({
  selector: 'pages-button',
  templateUrl: './pages-button.component.html',
  standalone: true,
  imports: [
    NgClass,
    NgIf
  ],
  styleUrl: './pages-button.component.css'
})
export class PagesButton implements OnInit {
  @Input() collapsed!: boolean;
  @Input() buttonText: string = 'Non défini';
  @Input() iconClass: string = 'fa-solid fa-x';
  isSelected: boolean = false;

  constructor(private router: Router) {}

  ngOnInit() {
    this.isSelected = this.router.url.includes(`/${this.buttonText.toLowerCase()}`);

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd) // Filtre nécessaire pour ne pas spam le check.
    ).subscribe(() => {
      this.isSelected = this.router.url.includes(`/${this.buttonText.toLowerCase()}`);
    });
  }

  protected readonly C = C;
}
