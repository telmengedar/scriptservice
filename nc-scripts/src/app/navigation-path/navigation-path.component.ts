import { Component, Input } from '@angular/core';
import { NavigationItem } from '../dto/navigation/navigationItem';
import { Router } from '@angular/router';

/**
 * provides an inpage navigation to content pages
 */
@Component({
  selector: 'app-navigation-path',
  templateUrl: './navigation-path.component.html',
  styleUrls: ['./navigation-path.component.css']
})
export class NavigationPathComponent {
  @Input() path: NavigationItem[]=[];

  constructor(private router: Router) {
  }

  /**
   * navigates to the clicked item
   * @param item item to which to navigate
   */
  navigateTo(item: NavigationItem): void {
    if(!item.url)
      return;
    
    this.router.navigateByUrl(item.url);
  }
}
