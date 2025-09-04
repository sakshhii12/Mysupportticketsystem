import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AngularMaterialModule } from '../../../angular-material.module';

@Component({
  selector: 'app-assign-agent-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, AngularMaterialModule],
  templateUrl: './assign-agent-dialog.html',
})
export class AssignAgentDialog {
  agentEmail = '';
  constructor(public dialogRef: MatDialogRef<AssignAgentDialog>) { }
  onAssign(): void { this.dialogRef.close(this.agentEmail); }
  onCancel(): void { this.dialogRef.close(); }
}
