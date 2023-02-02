import { ImageUpload } from "./index";

export default {
    title: 'ImageUpload',
    component: ImageUpload
}

const Template = (args) => <ImageUpload input={{}} meta={{}} {...args} />


export const Basic = Template.bind({})
Basic.args = {
    text: {
        label: 'Upload an image'
    },
}