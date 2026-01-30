import { Component, Inject, inject, input, OnInit, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';

import { TextInput } from "../../../shared/text-input/text-input";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register  {
  

//  child => parent
//   membersFromHome = input.required<User[]>();


private accountService = inject(AccountService);

private router = inject(Router);

private fb= inject(FormBuilder);

   cancelRegister = output<boolean>();

protected creds= {} as RegisterCreds;

//protected registerForm:FormGroup;

protected credentialsForm:FormGroup;
protected profileForm:FormGroup;
protected currentStep= signal(1);

protected validationErrors= signal<string[]>([]); 

constructor() {
  this.credentialsForm = this.fb.group({ 
  email: ['', [Validators.required, Validators.email]],
  displayName:['', Validators.required],
  password:['', [Validators.required, Validators.minLength(4),Validators.maxLength(8)]],
  
  confirmPassword: ['', [Validators.required, this.matchValues('password')]]

});

this.profileForm = this.fb.group({
  gender: ['male',Validators.required],
  dateOfBirth: ['',Validators.required],
  city: ['',Validators.required],
  country: ['',Validators.required],
});

this.credentialsForm.controls['password'].valueChanges.subscribe({
  next: () => {
    this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
  } })
  
}




matchValues(matchTo:string):ValidatorFn{

  return (control:AbstractControl):ValidationErrors| null => {
 
      const parent = control.parent;
    if(!parent) return null;
    
    const matchValue = parent.get(matchTo)?.value;

      return control.value === matchValue ? null : {passwordMismatch:true};
      
    
    
  }
}

nextStep(){
  if(this.credentialsForm.valid){
    this.currentStep.update(n => n + 1);
  }   
}

prevStep(){
  this.currentStep.update(n => n - 1);
}


getMaxDate(){
  const today = new Date();
  today.setFullYear(today.getFullYear() - 18);
  return today.toISOString().split('T')[0];
}


register(){


  if(this.credentialsForm.valid && this.profileForm.valid){

    const formData={...this.credentialsForm.value,...this.profileForm.value};

  //  console.log('Form data:', formData);
this.accountService.register(formData).subscribe({

next: () => {

  this.router.navigateByUrl('/members');

},
error: error=> {console.log(error);
this.validationErrors.set(error);


  }})


}}

cancel(){

  this.cancelRegister.emit(false);
}

}
