import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveService } from '../services/hive.service';
import { Hive } from '../models/hive';
import { UpdateHiveRequest } from '../models/update-hive-request';

@Component({
  selector: 'app-hive-form',
  templateUrl: './hive-form.component.html',
  styleUrls: ['./hive-form.component.css']
})
export class HiveFormComponent implements OnInit {

  hive = new Hive(0, "", "", "", false, "");
  existed = false;
  updateHiveRequest = new UpdateHiveRequest("","","");
  hiveId = 0;
  lastUpdate = "";

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveService: HiveService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      if (p['id'] === undefined) return;
      this.hiveService.getHive(p['id']).subscribe(h => this.hive = h);
      this.existed = true;
      this.hiveId = p['id'];
    });
  }

  navigateToHives() {
    this.router.navigate(['/hives']);
  }

  onCancel() {
    this.navigateToHives();
  }
  
  onSubmit() {
    this.updateHiveRequest.name = this.hive.name;
    this.updateHiveRequest.code = this.hive.code;
    this.updateHiveRequest.address = this.hive.address;

    if(!this.existed)
    {
       return this.hiveService.addHive(this.updateHiveRequest)
        .subscribe(h => this.navigateToHives());
    }
    else
    {
      return this.hiveService.updateHive(this.hive.id, this.updateHiveRequest)
        .subscribe(h => this.navigateToHives());
    }
  }

  onDelete() {
    this.hiveService.setHiveStatus(this.hive.id, true).subscribe(c => this.hive.isDeleted = true);
  }

  onUndelete() {
    return this.hiveService.setHiveStatus(this.hive.id, false).subscribe(c => this.hive.isDeleted = false);
  }

  onPurge() {
    this.hiveService.deleteHive(this.hive.id).subscribe(p => this.navigateToHives());
  }

  onClear() {
    this.hiveId = this.hive.id;
    this.lastUpdate = this.hive.lastUpdated;
    if(this.hiveId == 0) {
      this.hiveId = this.hive.id;
    }
    this.hive = new Hive(this.hiveId,"","","",false,this.lastUpdate);
  }
}
