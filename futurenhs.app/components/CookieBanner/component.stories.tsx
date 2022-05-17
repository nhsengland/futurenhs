import { CookieBanner } from './index'

export default {
    title: 'CookieBanner',
    component: CookieBanner,
    argTypes: {
        text: {
            control: { type: '' }
        }
    }
}

const Template = (args) => <CookieBanner {...args} />

export const Basic = Template.bind({});
Basic.args = {
    cookieName: Date.now().toString()
}