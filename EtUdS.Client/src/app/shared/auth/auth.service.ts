import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements CanActivate {
  // Permet le chargement
  public isLoading = new BehaviorSubject<boolean>(false);
  private fakeLoadingTime: number = 300; // 1000 = 1 sec. Temps de chargement pour compréhension que quelque chose se passe...

  constructor(private https: HttpClient, private router: Router) {
    this.checkSession();
  }

  canActivate(): Promise<boolean> {
    return new Promise((resolve) => {
        if(window.location.pathname === '/connexion') {
          this.isLoading.next(true);
        }

        const etudiantId = this.getEtudiantId();
        if (!etudiantId) {
          this.router.navigate(['/connexion']);
          setTimeout(() => {
            this.isLoading.next(false);
          }, this.fakeLoadingTime);
          resolve(false);
        } else {
          setTimeout(() => {
            this.isLoading.next(false);
          }, this.fakeLoadingTime);
          resolve(true);
        }
    });
  }

  getEtudiant(): Observable<{ id: number; name: string }> {
    if(localStorage.getItem('etudiantId') == null || localStorage.getItem('nom') == null) {
      const host = window.location.hostname;
      let url = `https://${host}/api/Connexion/Utilisateur`;

      return this.https.get<{ id: number; name: string }>(url, {withCredentials: true}).pipe(
        tap((result) => {
          localStorage.setItem('etudiantId', result.id.toString());
          localStorage.setItem('nom', result.name.toString());
        })
      );
    } else {
      return new Observable<{ id: number, name: string }>((observer) => {
        const id = localStorage.getItem('etudiantId');
        const name = localStorage.getItem('nom');

        if (id && name) {
          observer.next({ id: parseInt(id), name });
        } else {
          observer.error('Erreur, aucune valeur dans le stockage (impossible en ce point).');
        }
        observer.complete();
      });
    }
  }

  getEtudiantId(): number | null {
    const id = localStorage.getItem('etudiantId');
    return id !== null ? parseInt(id) : null;
  }

  logout() {
    this.isLoading.next(true);
    const host = window.location.hostname;

    let url = `https://${host}/api/Connexion/Logout`;
    this.https.post(url, {}, { withCredentials: true }).subscribe(
      () => {
        localStorage.removeItem('etudiantId');
        localStorage.removeItem('nom');
        setTimeout(() => {
          this.isLoading.next(false);
        }, this.fakeLoadingTime);
      },
      (error) => {
        console.error('Erreur:', error);
        this.isLoading.next(false);
      }
    );

    this.router.navigate(['/connexion']);
  }

  checkSession() {
    if (window.location.pathname !== '/connexion' && window.location.pathname !== '/inscription') {
      this.isLoading.next(true);

      const host = window.location.hostname;
      let url = `https://${host}/api/Connexion/Utilisateur`;
      this.https
        .get<{ id: number; name: string }>(url, { withCredentials: true })
        .subscribe(
          (result) => {
            setTimeout(() => {
              this.isLoading.next(false);
            }, this.fakeLoadingTime);
          },
          (error) => {
            this.isLoading.next(false);
            if (error.status === 401) {
              this.router.navigate(['/connexion']);
            }
          }
        );
    }
  }
}
