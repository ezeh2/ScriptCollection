import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommitChange } from '../git'; // Adjusted path

@Component({
  selector: 'app-change-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './change-list.html',
  styleUrl: './change-list.css'
})
export class ChangeListComponent {
  @Input() changes: CommitChange[] = [];
}
