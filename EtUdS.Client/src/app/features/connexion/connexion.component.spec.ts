import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Connexion } from './connexion.component';

describe('Connexion', () => {
  let component: Connexion;
  let fixture: ComponentFixture<Connexion>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Connexion]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Connexion);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
