import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { layoutIds } from '@constants/routes';
import { routes } from '@jestMocks/generic-props';
import Page, { getServerSideProps } from './index.page';

import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const props: Props = {
    id: 'mockPageId',
    routes: routes,
    layoutId: layoutIds.BASE,
    user: undefined,
    contentText: {
        title: 'mockTitle',
        metaDescription: 'mockMetaDescriptionText',
        mainHeading: 'mockMainHeading',
    }
};

describe('Privacy policy page', () => {

    it('renders correctly', () => {

        render(<Page {...props} />);

        expect(screen.getAllByText('mockMainHeading').length).toEqual(1);

    });

    it('gets required server side props', async () => {

        const serverSideProps = await getServerSideProps({} as any);
                
        expect(serverSideProps).toHaveProperty('props.contentText');

    });
    
});
