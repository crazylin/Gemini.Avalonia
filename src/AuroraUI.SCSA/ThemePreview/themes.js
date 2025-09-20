// SCSA 主题预览器 - 主题定义
const themes = {
    'modern-blue': {
        name: '现代蓝色',
        description: '专业、清新的蓝色主题，适合长时间工作',
        variables: {
            // 主色调
            '--primary-color': '#4f7cff',
            '--primary-color-alpha': 'rgba(79, 124, 255, 0.15)',
            '--secondary-color': '#6c7ce7',
            '--accent-color': '#00d4ff',
            
            // 背景色
            '--background-color': '#fafbfc',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            // 文字颜色
            '--text-primary': '#2c3e50',
            '--text-secondary': '#7f8c8d',
            '--text-muted': '#bdc3c7',
            
            // 边框颜色
            '--border-color': '#e1e8ed',
            '--hover-color': 'rgba(79, 124, 255, 0.05)',
            
            // 状态颜色
            '--success-color': '#00d68f',
            '--success-color-alpha': 'rgba(0, 214, 143, 0.2)',
            '--warning-color': '#ffb946',
            '--error-color': '#ff4757',
            '--info-color': '#3742fa',
            
            // 状态背景色
            '--success-bg': 'rgba(0, 214, 143, 0.1)',
            '--success-text': '#00a085',
            '--warning-bg': 'rgba(255, 185, 70, 0.1)',
            '--warning-text': '#cc7a00',
            '--error-bg': 'rgba(255, 71, 87, 0.1)',
            '--error-text': '#cc1e2e',
            '--info-bg': 'rgba(55, 66, 250, 0.1)',
            '--info-text': '#1e3ad1',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #667eea 0%, #764ba2 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.8);
            }
            .nav-link:hover {
                background: rgba(255, 255, 255, 0.1);
                color: white;
            }
            .nav-link.active {
                background: rgba(255, 255, 255, 0.2);
                color: white;
            }
            .logo {
                background: rgba(255, 255, 255, 0.1);
                color: white;
            }
            .header {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
            }
            .card {
                box-shadow: 0 2px 12px rgba(79, 124, 255, 0.08);
            }
        `
    },

    'dark-professional': {
        name: '专业深色',
        description: '经典深色主题，减少眼部疲劳，适合夜间工作',
        variables: {
            '--primary-color': '#64ffda',
            '--primary-color-alpha': 'rgba(100, 255, 218, 0.15)',
            '--secondary-color': '#7c4dff',
            '--accent-color': '#ff6b6b',
            
            '--background-color': '#121212',
            '--surface-color': '#1e1e1e',
            '--card-background': '#2d2d2d',
            
            '--text-primary': '#ffffff',
            '--text-secondary': '#b0b0b0',
            '--text-muted': '#6a6a6a',
            
            '--border-color': '#404040',
            '--hover-color': 'rgba(100, 255, 218, 0.08)',
            
            '--success-color': '#4caf50',
            '--success-color-alpha': 'rgba(76, 175, 80, 0.2)',
            '--warning-color': '#ff9800',
            '--error-color': '#f44336',
            '--info-color': '#2196f3',
            
            '--success-bg': 'rgba(76, 175, 80, 0.15)',
            '--success-text': '#81c784',
            '--warning-bg': 'rgba(255, 152, 0, 0.15)',
            '--warning-text': '#ffb74d',
            '--error-bg': 'rgba(244, 67, 54, 0.15)',
            '--error-text': '#e57373',
            '--info-bg': 'rgba(33, 150, 243, 0.15)',
            '--info-text': '#64b5f6',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #2c3e50 0%, #34495e 100%);
                color: white;
                border-right-color: #404040;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.8);
            }
            .nav-link:hover {
                background: rgba(100, 255, 218, 0.1);
                color: var(--primary-color);
            }
            .nav-link.active {
                background: rgba(100, 255, 218, 0.15);
                color: var(--primary-color);
            }
            .logo {
                background: rgba(100, 255, 218, 0.1);
                color: var(--primary-color);
            }
            .header {
                background: var(--card-background);
                color: var(--text-primary);
            }
            .header h1 {
                color: var(--text-primary);
            }
            .btn-primary {
                background: var(--primary-color);
                color: #121212;
            }
            .btn-secondary {
                background: var(--surface-color);
                color: var(--text-primary);
                border-color: var(--border-color);
            }
            .card {
                background: var(--card-background);
                border-color: var(--border-color);
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            }
            .card h3 {
                color: var(--text-primary);
            }
            .form-input, .form-select {
                background: var(--surface-color);
                color: var(--text-primary);
                border-color: var(--border-color);
            }
            .form-label {
                color: var(--text-secondary);
            }
            .data-table th {
                background: var(--surface-color);
                color: var(--text-primary);
            }
            .metric-value {
                color: var(--text-primary);
            }
            .metric-label {
                color: var(--text-secondary);
            }
        `
    },

    'nature-green': {
        name: '自然绿色',
        description: '清新自然的绿色主题，营造宁静专注的工作环境',
        variables: {
            '--primary-color': '#27ae60',
            '--primary-color-alpha': 'rgba(39, 174, 96, 0.15)',
            '--secondary-color': '#2ecc71',
            '--accent-color': '#f39c12',
            
            '--background-color': '#f8fffe',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#2c3e50',
            '--text-secondary': '#34495e',
            '--text-muted': '#7f8c8d',
            
            '--border-color': '#e8f5e8',
            '--hover-color': 'rgba(39, 174, 96, 0.05)',
            
            '--success-color': '#27ae60',
            '--success-color-alpha': 'rgba(39, 174, 96, 0.2)',
            '--warning-color': '#f39c12',
            '--error-color': '#e74c3c',
            '--info-color': '#3498db',
            
            '--success-bg': 'rgba(39, 174, 96, 0.1)',
            '--success-text': '#229954',
            '--warning-bg': 'rgba(243, 156, 18, 0.1)',
            '--warning-text': '#d68910',
            '--error-bg': 'rgba(231, 76, 60, 0.1)',
            '--error-text': '#cb4335',
            '--info-bg': 'rgba(52, 152, 219, 0.1)',
            '--info-text': '#2e86c1',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #56ab2f 0%, #a8e6cf 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.9);
            }
            .nav-link:hover {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .nav-link.active {
                background: rgba(255, 255, 255, 0.25);
                color: white;
            }
            .logo {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .header {
                background: linear-gradient(135deg, #56ab2f 0%, #a8e6cf 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
            }
            .card {
                box-shadow: 0 2px 12px rgba(39, 174, 96, 0.1);
                border-left: 3px solid var(--primary-color);
            }
        `
    },

    'sunset-orange': {
        name: '夕阳橙色',
        description: '温暖活力的橙色主题，激发创造力和热情',
        variables: {
            '--primary-color': '#ff6b35',
            '--primary-color-alpha': 'rgba(255, 107, 53, 0.15)',
            '--secondary-color': '#f39c12',
            '--accent-color': '#e74c3c',
            
            '--background-color': '#fffaf7',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#2c3e50',
            '--text-secondary': '#5d4e75',
            '--text-muted': '#95a5a6',
            
            '--border-color': '#ffeee6',
            '--hover-color': 'rgba(255, 107, 53, 0.05)',
            
            '--success-color': '#2ecc71',
            '--success-color-alpha': 'rgba(46, 204, 113, 0.2)',
            '--warning-color': '#f39c12',
            '--error-color': '#e74c3c',
            '--info-color': '#3498db',
            
            '--success-bg': 'rgba(46, 204, 113, 0.1)',
            '--success-text': '#27ae60',
            '--warning-bg': 'rgba(243, 156, 18, 0.1)',
            '--warning-text': '#d68910',
            '--error-bg': 'rgba(231, 76, 60, 0.1)',
            '--error-text': '#cb4335',
            '--info-bg': 'rgba(52, 152, 219, 0.1)',
            '--info-text': '#2e86c1',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #ff7e5f 0%, #feb47b 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.9);
            }
            .nav-link:hover {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .nav-link.active {
                background: rgba(255, 255, 255, 0.25);
                color: white;
            }
            .logo {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .header {
                background: linear-gradient(135deg, #ff7e5f 0%, #feb47b 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
            }
            .card {
                box-shadow: 0 2px 12px rgba(255, 107, 53, 0.1);
            }
            .metric-card:nth-child(1) {
                background: linear-gradient(135deg, #ff7e5f20 0%, #feb47b20 100%);
            }
        `
    },

    'minimal-grey': {
        name: '极简灰色',
        description: '简约优雅的灰色主题，专注内容，减少干扰',
        variables: {
            '--primary-color': '#5a67d8',
            '--primary-color-alpha': 'rgba(90, 103, 216, 0.15)',
            '--secondary-color': '#667eea',
            '--accent-color': '#ed8936',
            
            '--background-color': '#f7fafc',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#1a202c',
            '--text-secondary': '#4a5568',
            '--text-muted': '#a0aec0',
            
            '--border-color': '#e2e8f0',
            '--hover-color': 'rgba(90, 103, 216, 0.05)',
            
            '--success-color': '#38a169',
            '--success-color-alpha': 'rgba(56, 161, 105, 0.2)',
            '--warning-color': '#d69e2e',
            '--error-color': '#e53e3e',
            '--info-color': '#3182ce',
            
            '--success-bg': 'rgba(56, 161, 105, 0.1)',
            '--success-text': '#2f855a',
            '--warning-bg': 'rgba(214, 158, 46, 0.1)',
            '--warning-text': '#b7791f',
            '--error-bg': 'rgba(229, 62, 62, 0.1)',
            '--error-text': '#c53030',
            '--info-bg': 'rgba(49, 130, 206, 0.1)',
            '--info-text': '#2c5282',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #718096 0%, #4a5568 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.8);
            }
            .nav-link:hover {
                background: rgba(255, 255, 255, 0.1);
                color: white;
            }
            .nav-link.active {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .logo {
                background: rgba(255, 255, 255, 0.1);
                color: white;
            }
            .header {
                background: var(--surface-color);
                color: var(--text-primary);
                border-bottom: 1px solid var(--border-color);
            }
            .btn-primary {
                background: var(--primary-color);
            }
            .card {
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
                border: 1px solid var(--border-color);
            }
        `
    },

    'high-contrast': {
        name: '高对比度',
        description: '高对比度主题，提高可读性，适合视力敏感用户',
        variables: {
            '--primary-color': '#0066ff',
            '--primary-color-alpha': 'rgba(0, 102, 255, 0.15)',
            '--secondary-color': '#6600cc',
            '--accent-color': '#ff3300',
            
            '--background-color': '#ffffff',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#000000',
            '--text-secondary': '#333333',
            '--text-muted': '#666666',
            
            '--border-color': '#000000',
            '--hover-color': 'rgba(0, 102, 255, 0.1)',
            
            '--success-color': '#008800',
            '--success-color-alpha': 'rgba(0, 136, 0, 0.2)',
            '--warning-color': '#ff8800',
            '--error-color': '#cc0000',
            '--info-color': '#0066ff',
            
            '--success-bg': 'rgba(0, 136, 0, 0.1)',
            '--success-text': '#006600',
            '--warning-bg': 'rgba(255, 136, 0, 0.1)',
            '--warning-text': '#cc6600',
            '--error-bg': 'rgba(204, 0, 0, 0.1)',
            '--error-text': '#990000',
            '--info-bg': 'rgba(0, 102, 255, 0.1)',
            '--info-text': '#004499',
        },
        styles: `
            .sidebar {
                background: #000000;
                color: white;
                border-right: 2px solid #000000;
            }
            .nav-link {
                color: white;
                border: 1px solid transparent;
            }
            .nav-link:hover {
                background: #333333;
                color: white;
                border: 1px solid white;
            }
            .nav-link.active {
                background: var(--primary-color);
                color: white;
                border: 1px solid white;
            }
            .logo {
                background: #333333;
                color: white;
                border: 2px solid white;
            }
            .header {
                background: var(--surface-color);
                color: var(--text-primary);
                border: 2px solid #000000;
            }
            .btn-primary {
                background: var(--primary-color);
                border: 2px solid #000000;
            }
            .btn-secondary {
                background: white;
                color: #000000;
                border: 2px solid #000000;
            }
            .card {
                border: 2px solid #000000;
                box-shadow: 4px 4px 0 rgba(0, 0, 0, 0.3);
            }
            .form-input, .form-select {
                border: 2px solid #000000;
            }
            .form-input:focus {
                border: 2px solid var(--primary-color);
                box-shadow: 0 0 0 2px rgba(0, 102, 255, 0.3);
            }
            .data-table th,
            .data-table td {
                border: 1px solid #000000;
            }
        `
    },

    'ocean-blue': {
        name: '深海蓝色',
        description: '深邃海洋主题，沉稳专业，适合长期专注工作',
        variables: {
            '--primary-color': '#006ba6',
            '--primary-color-alpha': 'rgba(0, 107, 166, 0.15)',
            '--secondary-color': '#0085c7',
            '--accent-color': '#00a8cc',
            
            '--background-color': '#f0f8ff',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#1c3a57',
            '--text-secondary': '#2e5984',
            '--text-muted': '#7ba7d1',
            
            '--border-color': '#d4e8ff',
            '--hover-color': 'rgba(0, 107, 166, 0.08)',
            
            '--success-color': '#00695c',
            '--success-color-alpha': 'rgba(0, 105, 92, 0.2)',
            '--warning-color': '#ef6c00',
            '--error-color': '#c62828',
            '--info-color': '#1976d2',
            
            '--success-bg': 'rgba(0, 105, 92, 0.1)',
            '--success-text': '#00695c',
            '--warning-bg': 'rgba(239, 108, 0, 0.1)',
            '--warning-text': '#e65100',
            '--error-bg': 'rgba(198, 40, 40, 0.1)',
            '--error-text': '#c62828',
            '--info-bg': 'rgba(25, 118, 210, 0.1)',
            '--info-text': '#1565c0',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #0d47a1 0%, #1565c0 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.9);
            }
            .nav-link:hover {
                background: rgba(255, 255, 255, 0.1);
                color: white;
            }
            .nav-link.active {
                background: rgba(255, 255, 255, 0.2);
                color: white;
                border-left: 3px solid #00a8cc;
            }
            .logo {
                background: rgba(255, 255, 255, 0.15);
                color: white;
            }
            .header {
                background: linear-gradient(135deg, #0d47a1 0%, #1976d2 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
                box-shadow: 0 4px 12px rgba(0, 107, 166, 0.3);
            }
            .card {
                box-shadow: 0 2px 20px rgba(0, 107, 166, 0.1);
                border-left: 4px solid var(--primary-color);
            }
        `
    },

    'cyberpunk-neon': {
        name: '赛博朋克',
        description: '未来科技风格，霓虹色彩，适合创新型工作环境',
        variables: {
            '--primary-color': '#00ffff',
            '--primary-color-alpha': 'rgba(0, 255, 255, 0.15)',
            '--secondary-color': '#ff00ff',
            '--accent-color': '#ffff00',
            
            '--background-color': '#0a0a0f',
            '--surface-color': '#141420',
            '--card-background': '#1a1a2e',
            
            '--text-primary': '#ffffff',
            '--text-secondary': '#c0c0ff',
            '--text-muted': '#8080c0',
            
            '--border-color': '#2a2a50',
            '--hover-color': 'rgba(0, 255, 255, 0.1)',
            
            '--success-color': '#00ff80',
            '--success-color-alpha': 'rgba(0, 255, 128, 0.2)',
            '--warning-color': '#ffaa00',
            '--error-color': '#ff0080',
            '--info-color': '#0080ff',
            
            '--success-bg': 'rgba(0, 255, 128, 0.15)',
            '--success-text': '#00ff80',
            '--warning-bg': 'rgba(255, 170, 0, 0.15)',
            '--warning-text': '#ffaa00',
            '--error-bg': 'rgba(255, 0, 128, 0.15)',
            '--error-text': '#ff0080',
            '--info-bg': 'rgba(0, 128, 255, 0.15)',
            '--info-text': '#0080ff',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #0f0f23 0%, #1a1a2e 100%);
                color: white;
                border-right: 2px solid #00ffff;
                box-shadow: 0 0 20px rgba(0, 255, 255, 0.3);
            }
            .nav-link {
                color: #c0c0ff;
            }
            .nav-link:hover {
                background: rgba(0, 255, 255, 0.1);
                color: #00ffff;
                text-shadow: 0 0 10px #00ffff;
            }
            .nav-link.active {
                background: rgba(0, 255, 255, 0.2);
                color: #00ffff;
                text-shadow: 0 0 10px #00ffff;
                border-left: 3px solid #00ffff;
            }
            .logo {
                background: rgba(0, 255, 255, 0.1);
                color: #00ffff;
                text-shadow: 0 0 15px #00ffff;
                border: 1px solid #00ffff;
            }
            .header {
                background: linear-gradient(135deg, #1a1a2e 0%, #2a2a50 100%);
                color: white;
                border: 1px solid #00ffff;
                box-shadow: 0 0 20px rgba(0, 255, 255, 0.2);
            }
            .header h1 {
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
                color: #0a0a0f;
                text-shadow: none;
                box-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
            }
            .card {
                background: var(--card-background);
                border: 1px solid #2a2a50;
                box-shadow: 0 4px 25px rgba(0, 255, 255, 0.1);
            }
            .card h3 {
                color: var(--text-primary);
            }
            .form-label {
                color: var(--text-secondary);
            }
            .metric-value {
                color: var(--text-primary);
            }
            .metric-label {
                color: var(--text-secondary);
            }
        `
    },

    'forest-dark': {
        name: '深林主题',
        description: '深色森林主题，自然沉静，适合需要专注的深度工作',
        variables: {
            '--primary-color': '#4caf50',
            '--primary-color-alpha': 'rgba(76, 175, 80, 0.15)',
            '--secondary-color': '#66bb6a',
            '--accent-color': '#8bc34a',
            
            '--background-color': '#1b2a1b',
            '--surface-color': '#2e3e2e',
            '--card-background': '#3a4a3a',
            
            '--text-primary': '#e8f5e8',
            '--text-secondary': '#c8e6c9',
            '--text-muted': '#81c784',
            
            '--border-color': '#4a5e4a',
            '--hover-color': 'rgba(76, 175, 80, 0.1)',
            
            '--success-color': '#66bb6a',
            '--success-color-alpha': 'rgba(102, 187, 106, 0.2)',
            '--warning-color': '#ffa726',
            '--error-color': '#ef5350',
            '--info-color': '#42a5f5',
            
            '--success-bg': 'rgba(102, 187, 106, 0.15)',
            '--success-text': '#81c784',
            '--warning-bg': 'rgba(255, 167, 38, 0.15)',
            '--warning-text': '#ffb74d',
            '--error-bg': 'rgba(239, 83, 80, 0.15)',
            '--error-text': '#e57373',
            '--info-bg': 'rgba(66, 165, 245, 0.15)',
            '--info-text': '#64b5f6',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #1b5e20 0%, #2e7d32 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.8);
            }
            .nav-link:hover {
                background: rgba(76, 175, 80, 0.2);
                color: white;
            }
            .nav-link.active {
                background: rgba(76, 175, 80, 0.3);
                color: white;
                border-left: 3px solid #8bc34a;
            }
            .logo {
                background: rgba(76, 175, 80, 0.2);
                color: white;
            }
            .header {
                background: var(--card-background);
                color: var(--text-primary);
                border-bottom: 2px solid var(--primary-color);
            }
            .header h1 {
                color: var(--text-primary);
            }
            .btn-primary {
                background: var(--primary-color);
                box-shadow: 0 4px 15px rgba(76, 175, 80, 0.4);
            }
            .card {
                background: var(--card-background);
                border: 1px solid var(--border-color);
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            }
            .card h3 {
                color: var(--text-primary);
            }
            .form-label {
                color: var(--text-secondary);
            }
            .metric-value {
                color: var(--text-primary);
            }
            .metric-label {
                color: var(--text-secondary);
            }
        `
    },

    'corporate-gold': {
        name: '企业金色',
        description: '高端企业风格，金色点缀，体现专业与品质',
        variables: {
            '--primary-color': '#b8860b',
            '--primary-color-alpha': 'rgba(184, 134, 11, 0.15)',
            '--secondary-color': '#daa520',
            '--accent-color': '#ffd700',
            
            '--background-color': '#fafaf8',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#2c2416',
            '--text-secondary': '#5d4e37',
            '--text-muted': '#8b7355',
            
            '--border-color': '#e8e2d4',
            '--hover-color': 'rgba(184, 134, 11, 0.05)',
            
            '--success-color': '#228b22',
            '--success-color-alpha': 'rgba(34, 139, 34, 0.2)',
            '--warning-color': '#ff8c00',
            '--error-color': '#dc143c',
            '--info-color': '#4682b4',
            
            '--success-bg': 'rgba(34, 139, 34, 0.1)',
            '--success-text': '#228b22',
            '--warning-bg': 'rgba(255, 140, 0, 0.1)',
            '--warning-text': '#ff8c00',
            '--error-bg': 'rgba(220, 20, 60, 0.1)',
            '--error-text': '#dc143c',
            '--info-bg': 'rgba(70, 130, 180, 0.1)',
            '--info-text': '#4682b4',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #2c2416 0%, #5d4e37 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.9);
            }
            .nav-link:hover {
                background: rgba(218, 165, 32, 0.2);
                color: #ffd700;
            }
            .nav-link.active {
                background: rgba(218, 165, 32, 0.3);
                color: #ffd700;
                border-left: 3px solid #ffd700;
            }
            .logo {
                background: rgba(218, 165, 32, 0.2);
                color: #ffd700;
            }
            .header {
                background: linear-gradient(135deg, #2c2416 0%, #5d4e37 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
                color: white;
                box-shadow: 0 4px 15px rgba(184, 134, 11, 0.3);
            }
            .card {
                box-shadow: 0 3px 15px rgba(184, 134, 11, 0.1);
                border-top: 3px solid var(--primary-color);
            }
        `
    },

    'medical-clean': {
        name: '医疗洁净',
        description: '医疗级洁净主题，简洁可靠，适合精密仪器操作界面',
        variables: {
            '--primary-color': '#0277bd',
            '--primary-color-alpha': 'rgba(2, 119, 189, 0.15)',
            '--secondary-color': '#0288d1',
            '--accent-color': '#039be5',
            
            '--background-color': '#f8fffe',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#263238',
            '--text-secondary': '#455a64',
            '--text-muted': '#78909c',
            
            '--border-color': '#e0f2f1',
            '--hover-color': 'rgba(2, 119, 189, 0.04)',
            
            '--success-color': '#00695c',
            '--success-color-alpha': 'rgba(0, 105, 92, 0.2)',
            '--warning-color': '#f57c00',
            '--error-color': '#d32f2f',
            '--info-color': '#1976d2',
            
            '--success-bg': 'rgba(0, 105, 92, 0.08)',
            '--success-text': '#00695c',
            '--warning-bg': 'rgba(245, 124, 0, 0.08)',
            '--warning-text': '#f57c00',
            '--error-bg': 'rgba(211, 47, 47, 0.08)',
            '--error-text': '#d32f2f',
            '--info-bg': 'rgba(25, 118, 210, 0.08)',
            '--info-text': '#1976d2',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #eceff1 0%, #cfd8dc 100%);
                color: #263238;
                border-right: 1px solid #b0bec5;
            }
            .nav-link {
                color: #455a64;
            }
            .nav-link:hover {
                background: rgba(2, 119, 189, 0.08);
                color: var(--primary-color);
            }
            .nav-link.active {
                background: rgba(2, 119, 189, 0.12);
                color: var(--primary-color);
                border-left: 4px solid var(--primary-color);
            }
            .logo {
                background: rgba(2, 119, 189, 0.1);
                color: var(--primary-color);
            }
            .header {
                background: var(--surface-color);
                color: var(--text-primary);
                border-bottom: 1px solid var(--border-color);
            }
            .btn-primary {
                background: var(--primary-color);
                box-shadow: 0 2px 8px rgba(2, 119, 189, 0.2);
            }
            .card {
                box-shadow: 0 1px 8px rgba(0, 0, 0, 0.05);
                border: 1px solid var(--border-color);
            }
        `
    },

    'retro-terminal': {
        name: '复古终端',
        description: '经典终端风格，绿色磷光屏效果，怀旧极客风格',
        variables: {
            '--primary-color': '#00ff41',
            '--primary-color-alpha': 'rgba(0, 255, 65, 0.15)',
            '--secondary-color': '#39ff14',
            '--accent-color': '#7fff00',
            
            '--background-color': '#000000',
            '--surface-color': '#0a0a0a',
            '--card-background': '#111111',
            
            '--text-primary': '#00ff41',
            '--text-secondary': '#39ff14',
            '--text-muted': '#7fff00',
            
            '--border-color': '#003300',
            '--hover-color': 'rgba(0, 255, 65, 0.1)',
            
            '--success-color': '#00ff41',
            '--success-color-alpha': 'rgba(0, 255, 65, 0.2)',
            '--warning-color': '#ffff00',
            '--error-color': '#ff4500',
            '--info-color': '#00ffff',
            
            '--success-bg': 'rgba(0, 255, 65, 0.1)',
            '--success-text': '#00ff41',
            '--warning-bg': 'rgba(255, 255, 0, 0.1)',
            '--warning-text': '#ffff00',
            '--error-bg': 'rgba(255, 69, 0, 0.1)',
            '--error-text': '#ff4500',
            '--info-bg': 'rgba(0, 255, 255, 0.1)',
            '--info-text': '#00ffff',
        },
        styles: `
            .sidebar {
                background: #000000;
                color: #00ff41;
                border-right: 1px solid #003300;
                font-family: 'Courier New', monospace;
            }
            .nav-link {
                color: #39ff14;
                text-shadow: 0 0 5px #00ff41;
            }
            .nav-link:hover {
                background: rgba(0, 255, 65, 0.1);
                color: #00ff41;
                text-shadow: 0 0 10px #00ff41;
            }
            .nav-link.active {
                background: rgba(0, 255, 65, 0.2);
                color: #00ff41;
                text-shadow: 0 0 10px #00ff41;
                border-left: 3px solid #00ff41;
            }
            .logo {
                background: rgba(0, 255, 65, 0.1);
                color: #00ff41;
                text-shadow: 0 0 15px #00ff41;
                font-family: 'Courier New', monospace;
            }
            .header {
                background: var(--card-background);
                color: var(--text-primary);
                font-family: 'Courier New', monospace;
            }
            .header h1 {
                color: var(--text-primary);
                text-shadow: 0 0 10px #00ff41;
            }
            .btn-primary {
                background: var(--primary-color);
                color: #000000;
                text-shadow: none;
                box-shadow: 0 0 15px rgba(0, 255, 65, 0.5);
            }
            .card {
                background: var(--card-background);
                border: 1px solid #003300;
                box-shadow: 0 0 20px rgba(0, 255, 65, 0.1);
            }
            .card h3 {
                color: var(--text-primary);
                text-shadow: 0 0 5px #00ff41;
                font-family: 'Courier New', monospace;
            }
            .form-label {
                color: var(--text-secondary);
                font-family: 'Courier New', monospace;
            }
            .metric-value {
                color: var(--text-primary);
                text-shadow: 0 0 10px #00ff41;
                font-family: 'Courier New', monospace;
            }
            .metric-label {
                color: var(--text-secondary);
                font-family: 'Courier New', monospace;
            }
        `
    },

    'royal-purple': {
        name: '皇家紫色',
        description: '高贵典雅的紫色主题，彰显品味与格调',
        variables: {
            '--primary-color': '#673ab7',
            '--primary-color-alpha': 'rgba(103, 58, 183, 0.15)',
            '--secondary-color': '#7c4dff',
            '--accent-color': '#9c27b0',
            
            '--background-color': '#faf8ff',
            '--surface-color': '#ffffff',
            '--card-background': '#ffffff',
            
            '--text-primary': '#4a148c',
            '--text-secondary': '#6a1b9a',
            '--text-muted': '#9575cd',
            
            '--border-color': '#e1bee7',
            '--hover-color': 'rgba(103, 58, 183, 0.06)',
            
            '--success-color': '#388e3c',
            '--success-color-alpha': 'rgba(56, 142, 60, 0.2)',
            '--warning-color': '#f57c00',
            '--error-color': '#d32f2f',
            '--info-color': '#1976d2',
            
            '--success-bg': 'rgba(56, 142, 60, 0.1)',
            '--success-text': '#388e3c',
            '--warning-bg': 'rgba(245, 124, 0, 0.1)',
            '--warning-text': '#f57c00',
            '--error-bg': 'rgba(211, 47, 47, 0.1)',
            '--error-text': '#d32f2f',
            '--info-bg': 'rgba(25, 118, 210, 0.1)',
            '--info-text': '#1976d2',
        },
        styles: `
            .sidebar {
                background: linear-gradient(180deg, #4a148c 0%, #6a1b9a 100%);
                color: white;
            }
            .nav-link {
                color: rgba(255, 255, 255, 0.9);
            }
            .nav-link:hover {
                background: rgba(156, 39, 176, 0.2);
                color: white;
            }
            .nav-link.active {
                background: rgba(156, 39, 176, 0.3);
                color: white;
                border-left: 3px solid #e1bee7;
            }
            .logo {
                background: rgba(156, 39, 176, 0.2);
                color: white;
            }
            .header {
                background: linear-gradient(135deg, #4a148c 0%, #6a1b9a 100%);
                color: white;
            }
            .btn-primary {
                background: var(--primary-color);
                box-shadow: 0 4px 15px rgba(103, 58, 183, 0.4);
            }
            .card {
                box-shadow: 0 3px 15px rgba(103, 58, 183, 0.1);
                border-left: 4px solid var(--primary-color);
            }
        `
    },

    'arctic-white': {
        name: '极地白色',
        description: '极简纯净的白色主题，最大化内容可读性',
        variables: {
            '--primary-color': '#2196f3',
            '--primary-color-alpha': 'rgba(33, 150, 243, 0.15)',
            '--secondary-color': '#03a9f4',
            '--accent-color': '#00bcd4',
            
            '--background-color': '#ffffff',
            '--surface-color': '#fafafa',
            '--card-background': '#ffffff',
            
            '--text-primary': '#212121',
            '--text-secondary': '#424242',
            '--text-muted': '#757575',
            
            '--border-color': '#e0e0e0',
            '--hover-color': 'rgba(33, 150, 243, 0.04)',
            
            '--success-color': '#4caf50',
            '--success-color-alpha': 'rgba(76, 175, 80, 0.2)',
            '--warning-color': '#ff9800',
            '--error-color': '#f44336',
            '--info-color': '#2196f3',
            
            '--success-bg': 'rgba(76, 175, 80, 0.08)',
            '--success-text': '#2e7d32',
            '--warning-bg': 'rgba(255, 152, 0, 0.08)',
            '--warning-text': '#f57c00',
            '--error-bg': 'rgba(244, 67, 54, 0.08)',
            '--error-text': '#d32f2f',
            '--info-bg': 'rgba(33, 150, 243, 0.08)',
            '--info-text': '#1976d2',
        },
        styles: `
            .sidebar {
                background: #fafafa;
                color: #212121;
                border-right: 1px solid #e0e0e0;
            }
            .nav-link {
                color: #424242;
            }
            .nav-link:hover {
                background: rgba(33, 150, 243, 0.08);
                color: var(--primary-color);
            }
            .nav-link.active {
                background: rgba(33, 150, 243, 0.12);
                color: var(--primary-color);
                border-left: 3px solid var(--primary-color);
            }
            .logo {
                background: rgba(33, 150, 243, 0.08);
                color: var(--primary-color);
            }
            .header {
                background: var(--surface-color);
                color: var(--text-primary);
                border-bottom: 1px solid var(--border-color);
            }
            .btn-primary {
                background: var(--primary-color);
                box-shadow: 0 2px 8px rgba(33, 150, 243, 0.3);
            }
            .card {
                box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
                border: 1px solid var(--border-color);
            }
        `
    }
};

// 应用主题函数
function applyTheme(themeName) {
    const theme = themes[themeName];
    if (!theme) {
        console.error('Theme not found:', themeName);
        return;
    }

    // 移除现有主题样式
    const existingStyle = document.getElementById('current-theme');
    if (existingStyle) {
        existingStyle.remove();
    }

    // 创建新的样式元素
    const styleElement = document.createElement('style');
    styleElement.id = 'current-theme';
    
    // 应用CSS变量
    let cssText = ':root {\n';
    for (const [property, value] of Object.entries(theme.variables)) {
        cssText += `  ${property}: ${value};\n`;
    }
    cssText += '}\n\n';
    
    // 应用自定义样式
    cssText += theme.styles;
    
    styleElement.textContent = cssText;
    document.head.appendChild(styleElement);

    // 更新按钮状态
    document.querySelectorAll('.theme-btn').forEach(btn => btn.classList.remove('active'));
    event?.target.classList.add('active');

    // 输出主题信息到控制台（用于后续转换为Avalonia主题）
    console.log(`Applied theme: ${theme.name}`);
    console.log('Theme variables:', theme.variables);
    console.log('Theme description:', theme.description);
}

// 页面加载时应用默认主题
document.addEventListener('DOMContentLoaded', function() {
    applyTheme('modern-blue');
    
    // 添加主题切换快捷键
    document.addEventListener('keydown', function(e) {
        if (e.ctrlKey || e.metaKey) {
            const themeKeys = {
                '1': 'modern-blue',
                '2': 'dark-professional',
                '3': 'nature-green',
                '4': 'sunset-orange',
                '5': 'minimal-grey',
                '6': 'high-contrast'
            };
            
            if (themeKeys[e.key]) {
                e.preventDefault();
                applyTheme(themeKeys[e.key]);
            }
        }
    });
});

// 导出主题配置（用于后续转换为Avalonia主题）
function exportThemeForAvalonia(themeName) {
    const theme = themes[themeName];
    if (!theme) {
        console.error('Theme not found:', themeName);
        return;
    }
    
    console.log(`\n=== ${theme.name} - Avalonia XAML Theme ===`);
    console.log(`<!-- ${theme.description} -->`);
    console.log('<ResourceDictionary xmlns="https://github.com/avaloniaui"');
    console.log('                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">');
    console.log('\n  <!-- 颜色定义 -->');
    
    for (const [property, value] of Object.entries(theme.variables)) {
        const colorName = property.replace('--', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
        if (value.startsWith('#')) {
            console.log(`  <Color x:Key="${colorName}">${value}</Color>`);
        } else if (value.startsWith('rgba')) {
            // 转换 rgba 为 Avalonia 颜色格式
            const rgba = value.match(/rgba\((\d+),\s*(\d+),\s*(\d+),\s*([\d.]+)\)/);
            if (rgba) {
                const [, r, g, b, a] = rgba;
                const alpha = Math.round(parseFloat(a) * 255).toString(16).padStart(2, '0');
                const hex = `#${alpha}${parseInt(r).toString(16).padStart(2, '0')}${parseInt(g).toString(16).padStart(2, '0')}${parseInt(b).toString(16).padStart(2, '0')}`;
                console.log(`  <Color x:Key="${colorName}">${hex}</Color>`);
            }
        }
    }
    
    console.log('\n  <!-- 画刷定义 -->');
    for (const [property, value] of Object.entries(theme.variables)) {
        const brushName = property.replace('--', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase()) + 'Brush';
        const colorName = property.replace('--', '').replace(/-([a-z])/g, (g) => g[1].toUpperCase());
        if (value.startsWith('#') || value.startsWith('rgba')) {
            console.log(`  <SolidColorBrush x:Key="${brushName}" Color="{StaticResource ${colorName}}"/>`);
        }
    }
    
    console.log('\n</ResourceDictionary>');
}

// 添加到全局对象以便在控制台调用
window.exportThemeForAvalonia = exportThemeForAvalonia;
window.themes = themes;
