import { Component, OnInit, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSidenav } from '@angular/material/sidenav';

import { AngularMaterialModule } from '../../angular-material.module';
import { AuthService } from '../services/auth';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, AngularMaterialModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.css']
})
export class Layout implements OnInit {
  @ViewChild('drawer') drawer!: MatSidenav;

  userRoleIcon = 'account_circle';
  userRoleTooltip = 'User';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.removeAuthBackground();
    if (this.authService.isAdmin()) {
      this.userRoleIcon = 'admin_panel_settings';
      this.userRoleTooltip = 'Administrator';
    } else if (this.authService.isAgent()) {
      this.userRoleIcon = 'support_agent';
      this.userRoleTooltip = 'Agent';
    }
  }
  toggleDrawer(): void {
    this.drawer.toggle();
  }

  logout(): void {
    this.authService.logout();
  }
}
