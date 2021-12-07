export const logInFormFieldConfig = [
    {
        component: 'input',
        name: 'userName',
        content: {
            labelText: 'Email address'
        },
        validators: [
            {
                type: 'required',
                message: 'Enter your registered email address'
            }
        ]
    },
    {
        component: 'input',
        name: 'passWord',
        inputType: 'password',
        content: {
            labelText: 'Password'
        },
        validators: [
            {
                type: 'required',
                message: 'Enter your password'
            }
        ]
    }
]; 