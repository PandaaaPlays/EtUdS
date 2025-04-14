import { ComponentFixture, TestBed } from '@angular/core/testing';
import {EtUdSNavbar} from './etuds-navbar.component';


describe('NavbarComponent', () => {
  let component: EtUdSNavbar;
  let fixture: ComponentFixture<EtUdSNavbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EtUdSNavbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EtUdSNavbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
