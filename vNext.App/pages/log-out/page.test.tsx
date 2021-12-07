import * as React from 'react';
import { render, screen } from '@testing-library/react';

import Page, { getServerSideProps } from './index.page';

import { Props } from './interfaces';

const props: Props = {
    content: {
        titleText: 'mockTitle',
        metaDescriptionText: 'mockMetaDescriptionText',
        mainHeadingHtml: 'mockMainHeading',
    }
};

describe('Log out page', () => {

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });

    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({
            req: {
                cookie: 'mockCookie=value'
            }
        } as any);
                
        expect(serverSideProps).toHaveProperty('props.content');

    });
    
});
