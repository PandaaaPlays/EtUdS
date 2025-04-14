import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Membre } from './membre.component';

describe('Membre', () => {
  let component: Membre;
  let fixture: ComponentFixture<Membre>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Membre]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Membre);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
