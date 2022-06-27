import { create } from '@storybook/theming'
import { useAssetPath } from '../hooks/useAssetPath'

export default create({
    base: 'light',
    brandTitle: 'FutureNHS front end component library',
    brandUrl: 'https://collaborate.future.nhs.uk/',
    brandImage: useAssetPath('/images/logo.svg'),
    brandTarget: '_self',
})
