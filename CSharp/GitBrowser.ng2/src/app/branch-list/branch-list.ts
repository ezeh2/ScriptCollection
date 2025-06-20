import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Branch } from '../../git'; // Adjusted path

@Component({
  selector: 'app-branch-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './branch-list.html',
  styleUrl: './branch-list.css'
})
export class BranchListComponent {
  @Input() branches: Branch[] = [];
  @Input() selectedBranch: Branch | null = null; // To highlight the selected one
  @Output() branchSelected = new EventEmitter<Branch>();

  onSelect(branch: Branch): void {
    this.branchSelected.emit(branch);
  }
}
