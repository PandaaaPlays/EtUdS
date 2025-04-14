import { ComponentFixture, TestBed } from '@angular/core/testing';
import {ButtonEtuds} from './button-etuds.component';


describe('ButtonEtudsComponent', () => {
  let component: ButtonEtuds;
  let fixture: ComponentFixture<ButtonEtuds>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ButtonEtuds]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ButtonEtuds);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
