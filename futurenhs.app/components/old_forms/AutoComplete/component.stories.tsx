import { AutoComplete } from './index'

export default {
    title: 'AutoComplete',
    component: AutoComplete,
    argTypes: {
        input: {
            control: { type: '' },
        },
        meta: {
            control: { type: '' },
        },
        initialError: {
            control: { type: '' },
        },
        validators: {
            control: { type: '' },
        },
    },
}

const Template = (args) => (
    <AutoComplete input={{}} meta={{}} {...args} className="u-w-2/3" />
)

export const Basic = Template.bind({})
Basic.args = {
    input: {
        name: 'fieldName',
        onChange: () => {},
        onBlur: () => {},
        onFocus: () => {},
    },
    text: {
        label: 'Autocomplete input label',
        hint: 'Start typing a colour of the rainbow'
    },
    context: {
        user: {},
    },
    services: {
        autoCompleteService: ({ term }) => new Promise((resolve) => {

            const roygbiv: Array<string> = ['red', 'orange', 'yellow', 'green', 'blue', 'indigo', 'violet'];
            const matches: Array<{
                value: string;
                label: string;
            }> = []

            roygbiv.forEach((colour) => {

                if (colour.includes(term) || term.includes(colour)) {

                    matches.push({
                        value: colour,
                        label: colour
                    });

                }

            });

            console.log(matches);

            resolve({ data: matches });

        }) 
    },
    serviceId: 'autoCompleteService'
}
