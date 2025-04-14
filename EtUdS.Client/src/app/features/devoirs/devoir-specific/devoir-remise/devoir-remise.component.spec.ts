import { ComponentFixture, TestBed } from '@angular/core/testing';

import {DevoirRemise} from './devoir-remise.component';

describe('DevoirEquipe', () => {
  let component: DevoirRemise;
  let fixture: ComponentFixture<DevoirRemise>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DevoirRemise]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DevoirRemise);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
