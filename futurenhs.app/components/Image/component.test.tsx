import { render, screen } from "@testing-library/react";

import { Props } from './interfaces';

import { Image } from './index';

describe('Image', () => {

    const props: Props = {
        src: '/images/logo.svg',
        height: 100,
        width: 100,
        alt: 'Mock alt text',
        fallBack: {
            src: '/images/logo.svg',
            alt: 'Mock fallback alt text'
        }
    }

    it('renders image', () => {

        render(<Image {...props} />);

        expect(screen.getAllByAltText('Mock alt text').length).toBe(1);

    });

});