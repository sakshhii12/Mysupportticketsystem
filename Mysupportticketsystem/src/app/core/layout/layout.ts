import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AngularMaterialModule } from '../../angular-material.module';
import { AuthService } from '../services/auth'; 

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterModule, AngularMaterialModule],
  templateUrl: './layout.html',
  styleUrls: ['./layout.css']
})
export class Layout implements OnInit{

  userRoleIcon = 'person'; 
  userRoleTooltip = 'User';
  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.userRoleIcon = 'admin_panel_settings';
      this.userRoleTooltip = 'Administrator';
    } else if (this.authService.isAgent()) {
      this.userRoleIcon = 'support_agent';
      this.userRoleTooltip = 'Agent';
    }
  }
  logout(): void {
    this.authService.logout();
  }
}
