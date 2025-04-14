import {Component, ElementRef, Input, ViewChild} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {DialogComponent} from '../dialog/dialog.component';
import {MatDialog} from '@angular/material/dialog';

@Component({
  selector: 'drag-box',
  standalone: true,
  templateUrl: './drag-box.component.html',
  styleUrls: ['./drag-box.component.css'],
  imports: [
  ]
})

export class DragBox {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  @Input() action!: Function;

  constructor(private http: HttpClient, private dialog: MatDialog) {}

  onClick() {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.action(input.files);
    }
  }

  onDragEnter(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const dropZone = event.target as HTMLElement;
    dropZone.classList.add('drag-over');
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const dropZone = event.target as HTMLElement;
    dropZone.classList.remove('drag-over');
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const dropZone = event.target as HTMLElement;
    dropZone.classList.remove('drag-over');

    if (event.dataTransfer?.files.length) {
      this.action(event.dataTransfer.files);
    }
  }
}
