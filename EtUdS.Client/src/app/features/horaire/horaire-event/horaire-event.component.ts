import {AfterViewInit, ChangeDetectorRef, Component, ElementRef, Input, Renderer2, ViewChild} from '@angular/core';
import {NgClass, NgIf, NgStyle} from '@angular/common';
@Component({
  selector: 'horaire-event',
  standalone: true,

  templateUrl: './horaire-event.component.html',
  styleUrl: './horaire-event.component.css',
  imports: [
    NgStyle,
    NgIf,
    NgClass
  ]
})
export class HoraireEvent implements AfterViewInit{

  @ViewChild('titre') titreElement!: ElementRef;
  constructor(private renderer: Renderer2, private cdr: ChangeDetectorRef) {}

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.checkOverflow();
  }

  private checkOverflow() {
    const nativeElement = this.titreElement.nativeElement;
    if (nativeElement.scrollHeight > nativeElement.clientHeight) {
      this.renderer.setStyle(nativeElement, 'display', 'none');
    } else {
      this.renderer.setStyle(nativeElement, 'display', 'block');
    }
  }

  @Input() top: number = 0;
  @Input() height: number = 0;
  @Input() hour: string = '';
  @Input() hourEnd: string = '';
  @Input() sigle: string = '';
  @Input() nom: string = '';
  @Input() devoir: boolean = false;
  @Input() local: string = '';
}
