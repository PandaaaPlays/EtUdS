import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DevoirEquipe } from './devoir-equipe.component';

describe('DevoirEquipe', () => {
  let component: DevoirEquipe;
  let fixture: ComponentFixture<DevoirEquipe>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DevoirEquipe]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DevoirEquipe);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
