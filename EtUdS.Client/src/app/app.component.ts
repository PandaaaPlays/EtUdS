import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {BehaviorSubject, filter} from 'rxjs';
import {NavigationEnd, Router} from '@angular/router';
import {Title} from '@angular/platform-browser';
import {AuthService} from './shared/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  isConnectionPage = false;
  isCollapsed: boolean = true;
  loading$: BehaviorSubject<boolean> | undefined;

  constructor(private router: Router,
              private titleService: Title,
              private authService: AuthService,
              private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.isCollapsed = localStorage.getItem('isCollapsed') === 'true';
    this.loading$ = this.authService.isLoading;
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.isConnectionPage = this.router.url === '/connexion' || this.router.url === '/inscription';
      this.cdr.detectChanges();
    });

    const mois = new Date().getMonth() + 1;
    const session = mois <= 4 ? 'Hiver' : mois <= 8 ? 'Été' : 'Automne';
    this.titleService.setTitle(`ÉtUdS - ${session} ${new Date().getFullYear()}`);
  }

  toggleCollapse(value: boolean) {
    this.isCollapsed = value;
    localStorage.setItem('isCollapsed', String(value));
  }
}
