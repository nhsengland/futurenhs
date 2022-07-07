import { MultiChoice } from "./index";

export default {
    title: 'MultiChoice',
    component: MultiChoice,
    argTypes: {
        inputType: {
            control: { type: ''}
        },
        validators: {
            control: { type: '' } 
        }
    }
}

const Template = (args) => <MultiChoice input={{name:'role'}} meta={{}} {...args} />


export const Basic = Template.bind({})
Basic.args = {
    text: {
        label: 'Select users role'
    },
    options: [
        {
            name: 'Admin',
            label: 'Admin'
        },
        {
            name: 'Standard member',
            label: 'Standard member'
        },

    ]

}