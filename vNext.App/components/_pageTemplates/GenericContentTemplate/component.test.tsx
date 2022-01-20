import React from 'react';
import { render, screen } from '@testing-library/react';

import { GenericContentTemplate } from './index';

import { Props } from './interfaces';

const props: Props = {
    id: 'mockId',
    user: undefined,
    text: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    }
};

describe('Generic content template', () => {

    it('renders correctly', () => {

        render(<GenericContentTemplate {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });
    
});
