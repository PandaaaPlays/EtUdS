import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {PagesButton} from './pages-button/pages-button.component';
import {RouterLink} from '@angular/router';
import {AuthService} from '../auth/auth.service';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'etuds-navbar',
  standalone: true,
  templateUrl: './etuds-navbar.component.html',
  styleUrls: ['./etuds-navbar.component.css'],
  imports: [
    PagesButton,
    RouterLink,
    NgIf,
    NgClass,
  ]
})

export class EtUdSNavbar implements OnInit{
  @Input() isCollapsed!: boolean;
  @Output() isCollapsedChange = new EventEmitter<boolean>();
  nom!: string;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.getEtudiant().subscribe((etudiant) => {
        this.nom = etudiant.name;
      }
    );
  }

  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
    this.isCollapsedChange.emit(this.isCollapsed);
  }
}
