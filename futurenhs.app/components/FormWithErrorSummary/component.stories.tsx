import { FormConfig } from "@appTypes/form";
import { useState } from "react";
import { FormWithErrorSummary } from "./index";

export default {
    title: 'FormWithErrorSummary',
    component: FormWithErrorSummary,
    argTypes: {
        formConfig: {
            control: { type: '' }
        },
    }
}

const formConfig: FormConfig = {
    id: '123',
    steps: [
        {
            fields: [
                {
                    name: 'username',
                    inputType: 'text',
                    text: {
                        label: 'Username',
                        hint: 'Submit to see error summary'
                    },
                    component: 'input',
                }
            ]
        }
    ]
}

const Template = (args) => {
    
    const [errors, setErrors] = useState({})

    const handleSubmit = () => {
        setErrors({
            username: 'Enter a valid username'
        })
        return Promise.resolve({})
    }

    return (
        <div className="tablet:u-w-2/3">
            <FormWithErrorSummary 
                formConfig={formConfig}
                errors={errors}
                submitAction={handleSubmit}
                text={{
                    form: {
                        submitButton: 'Submit'
                    }
                }}
                csrfToken="789"
                {...args} 
            />
        </div>
    )
}

export const Basic = Template.bind({})